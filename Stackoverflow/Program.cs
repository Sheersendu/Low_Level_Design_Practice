using System.Collections.Concurrent;

class User(string name, string email) : IDisposable
{
	public Guid Id { get; } = Guid.NewGuid();
	public string Name { get; set; } = name;
	public string Email { get; set; } = email;
	private List<Question> Questions { get; } = new();
	private List<Answer> Answers { get; } = new();
	private ReaderWriterLockSlim _lock = new();

	public void AddQuestion(Question question)
	{
		_lock.EnterWriteLock();
		try
		{
			Questions.Add(question);
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}
	
	public List<Question> GetQuestions()
	{
		_lock.EnterReadLock();
		try
		{
			return Questions.ToList();
		}
		finally
		{
			_lock.ExitReadLock();
		}	
	}

	public void RemoveQuestion(Guid questionId)
	{
		_lock.EnterWriteLock();
		try
		{
			Questions.RemoveAll(question => question.Id == questionId);
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}

	public void AddAnswer(Answer answer)
	{
		_lock.EnterWriteLock();
		try
		{
			Answers.Add(answer);
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}
	
	public List<Answer> GetAnswers()
	{
		_lock.EnterReadLock();
		try
		{
			return Answers.ToList();
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}
	
	public void RemoveAnswer(Guid answerId)
	{
		_lock.EnterWriteLock();
		try
		{
			Answers.RemoveAll(answer => answer.Id == answerId);
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}

	public void Dispose()
	{
		_lock.Dispose();
	}
}
enum VoteType
{
	Up, Down
}

class Vote(Guid userId, VoteType vote)
{
	public Guid UserId = userId;
	public VoteType vote { get; } = vote;
}

interface IVotable
{
	void AddVote(Vote userVote);
	int GetTotalVotes();
}

class Comment(Guid userId, string description)
{
	private Guid Id { get; init; } = Guid.NewGuid();
	public Guid UserId { get; } = userId;
	public string Description { get; } = description;
	public DateTime CreatedAt { get; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

interface ICommentable
{
	void AddComment(Comment comment);
	List<Comment> GetAllComments();
}

abstract class Post(Guid userId, string description) : IVotable, ICommentable
{
	public Guid Id { get; } = Guid.NewGuid();
	public Guid UserId { get; } = userId;
	public string Description { get; } = description;
	private ConcurrentBag<Vote> _voteList = new ();
	private ConcurrentBag<Comment> _commentList = new();
	private ConcurrentDictionary<Guid, bool> _usersVoted = new();
	
	public void AddVote(Vote userVote)
	{
		if (_usersVoted.TryAdd(userVote.UserId, true))
		{
			_voteList.Add(userVote);
		}
	}
	
	public int GetTotalVotes()
	{
		int voteCount = 0;
		foreach (Vote vote in _voteList)
		{
			if (vote.vote == VoteType.Up)
			{
				voteCount += 1;
			}
			else
			{
				voteCount -= 1;
			}
		}

		return voteCount;
	}
	
	public void AddComment(Comment comment)
	{
		_commentList.Add(comment);
	}
	
	public List<Comment> GetAllComments()
	{
		return _commentList.OrderByDescending(c => c.UpdatedAt).ToList();
	}
}

class Question(Guid userId, string description) : Post(userId, description)
{
	private Guid BestAnswerId { get; set; }
	private List<Answer> AnswerList { get; } = new();
	private object _bestAnswerLock = new();

	public void SetBestAnswer(Guid answerId)
	{
		lock (_bestAnswerLock)
		{
			BestAnswerId = answerId;
		}
	}
	
	public Guid GetBestAnswer()
	{
		return BestAnswerId;
	}

	public void AddAnswer(Answer answer)
	{
		AnswerList.Add(answer);
	}
	
	public List<Answer> GetAllAnswers()
	{
		return AnswerList;
	}
}

class Answer(Guid userId, Guid questionId, string description) : Post(userId, description)
{
	public Guid QuestionId { get; set; } = questionId;
}

class StackOverflowSystem
{
	public StackOverflowSystem()
	{
		User u1 = new("User 1", "user1@email.com");
		User u2 = new("User 2", "user2@email.com");
		User u3 = new("User 3", "user3@email.com");
		User u4 = new("User 4", "user4@email.com");
		Question q1 = new Question(u1.Id, "Is this my question?");
		u1.AddQuestion(q1);
		
		Answer a1 = new Answer(u2.Id, q1.Id, "This is u1's questions's answer by U2!");
		q1.AddAnswer(a1);
		Vote u1Vote = new Vote(u1.Id, VoteType.Up);
		a1.AddVote(u1Vote);
		Comment comment = new Comment(u3.Id, "U3's comment on u1's question");
		q1.AddComment(comment);
		Answer a2 = new Answer(u4.Id, q1.Id, "This is u1's questions's answer by U4!");
		u4.AddAnswer(a2);
		q1.AddAnswer(a2);
		Vote u3Vote = new Vote(u4.Id, VoteType.Down);
		a2.AddVote(u3Vote);
		q1.SetBestAnswer(a1.Id);
		q1.AddVote(new Vote(u2.Id, VoteType.Up));
		q1.AddVote(new Vote(u3.Id, VoteType.Up));
		q1.AddVote(new Vote(u3.Id, VoteType.Up)); // Added to check if the vote is working as expected
		q1.AddVote(new Vote(u4.Id, VoteType.Down));

		foreach (var question in u1.GetQuestions())
		{
			Console.WriteLine($"Question: {question.Description}");
			foreach (var answer in question.GetAllAnswers())
			{
				Console.WriteLine($"{answer.Description} \t Total Votes: {answer.GetTotalVotes()}");
			}

			Console.WriteLine("----------------------------------------");
			foreach (var cmnt in question.GetAllComments())
			{
				Console.WriteLine(cmnt.Description);
			}

			Console.WriteLine("----------------------------------------");
			Console.WriteLine($"Total Votes: {question.GetTotalVotes()}");
			Console.WriteLine($"Best Answer: {question.GetBestAnswer()}, {question.GetBestAnswer() == a1.Id} ");
		}
	}
}

class StackOverflow
{
	public static void Main(string[] args)
	{
		new StackOverflowSystem();
	}
}