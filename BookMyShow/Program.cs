using System.Collections.Concurrent;
using System.Collections.Immutable;

class BaseClass
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}

class User(string name, string email) : BaseClass
{
	public string Name { get; set; } = name;
	public string Email { get; set; } = email;
}

class Movie(string name, string description, int duration) : BaseClass
{
	public string Name { get; set; } = name;
	public string Description { get; set; } = description;
	public int Duration { get; set; } = duration;
}

enum SeatType
{
	Gold, Silver
}

class Seat(string seatNo, SeatType type, double price) : BaseClass
{
	public string SeatNo { get; set; } = seatNo;
	public SeatType Type { get; set; } = type;
	public double Price { get; set; } = price;
	public bool IsAvailable { get; set; } = true;
	private object _status = new();

	public void Book()
	{
		lock (_status)
		{
			if (!IsAvailable)
			{
				Console.WriteLine("Seat already booked!");
				return;
			}
			IsAvailable = false;
		}
	}
}

class Show(Screen currentScreen, DateTime startTime, DateTime endTime, Movie movie) : BaseClass
{
	public Screen screen { get; set; } = currentScreen;
	public DateTime StartTime { get; set; } = startTime;
	public DateTime EndTime { get; set; } = endTime;
	private ConcurrentDictionary<Guid, Seat> _seatList = new();
	public Movie Movie { get; set; } = movie;

	public void AddSeat(Seat seat)
	{
		_seatList.TryAdd(seat.Id, seat);
	}

	public void RemoveSeat(Guid seatId)
	{
		_seatList.TryRemove(seatId, out _);
	}

	public Seat GetSeat(Guid seatId)
	{
		_seatList.TryGetValue(seatId, out var seat);
		return seat;
	}
	
	public List<Seat> GetAvailableSeatList(SeatType type)
	{
		return _seatList.Values.Where(seat => seat.Type == type && seat.IsAvailable).ToList();
	}
}

class Screen(string name) : BaseClass
{
	public string Name { get; set; } = name;
	private ConcurrentDictionary<Guid, Show> _showList = new();
	private readonly object _screenLock = new();

	public Show AddShow(DateTime startTime, DateTime endTime, Movie movie)
	{
		lock (_screenLock)
		{
			Show newShow = new(this, startTime, endTime, movie);
			_showList.TryAdd(newShow.Id, newShow);
			return newShow;
		}
	}
	
	public void RemoveShow(Guid showId)
	{
		lock (_screenLock)
		{
			_showList.Remove(showId, out _);
		}
	}

	public List<Show> GetAllShow()
	{
		lock (_screenLock)
		{
			return _showList.Values.ToList();
		}
	}

	public void DisplayShows()
	{
		foreach (var show in _showList.Values.OrderBy(show => show.StartTime).ToList())
		{
			Console.WriteLine($"Playing {show.Movie.Name} at {show.StartTime:HH:mm} - {show.EndTime:HH:mm}");
		}
		Console.WriteLine("---------------------------------------");
	}
}

class Theater(string name) : BaseClass
{
	public string Name { get; set; } = name;
	private ConcurrentDictionary<Guid, Screen> _screenList = new();
	
	public void AddScreen(Screen screen)
	{
		_screenList.TryAdd(screen.Id, screen);
	}
	
	public void RemoveScreen(Guid screenId)
	{
		_screenList.TryRemove(screenId, out _);
	}

	public List<Screen> GetAllScreen()
	{
		return _screenList.Values.ToList();
	}

	public void Display()
	{
		Console.WriteLine($"---------------------------------------");
		Console.WriteLine($"\t\t{Name}");
		Console.WriteLine($"---------------------------------------");
		foreach (var screen in _screenList.Values)
		{
			Console.WriteLine($"\t'{screen.Name}'");
			screen.DisplayShows();
		}
	}
}

enum PaymentStatus
{
	Success, Fail
}
	
class Payment : BaseClass
{
	public PaymentStatus Status { get; set; }
	private object _status = new();

	public void SetStatus(PaymentStatus status)
	{
		lock (_status)
		{
			Status = status;
		}
	}
}

class Booking : BaseClass
{
	public Guid UserId { get; }
	private string BookingId { get; init; } = new Random().Next().ToString();
	public ImmutableList<Seat> SelectedSeats { get; }
	private readonly Payment _payment = new ();
	private double _billAmount;
	private object _booking = new ();

	public Booking(Guid userId, ImmutableList<Seat> selectedSeats)
	{
		UserId = userId;
		_payment.SetStatus(PaymentStatus.Success);
		SelectedSeats = selectedSeats;
	}

	private void CalculateTotalAmount()
	{
		lock (_booking)
		{
			foreach (var seat in SelectedSeats)
			{
				seat.Book();
				_billAmount += seat.Price;
			}

			_billAmount = Math.Round(_billAmount, 2);
		}
	}
	
	public void MakePayment()
	{
		CalculateTotalAmount();
		Console.WriteLine($"Payment for Booking Id: {BookingId} for {_billAmount} was: {_payment.Status.ToString()}");
	}
}

class BookMyShowService
{
	private static BookMyShowService _instance;
	private readonly ConcurrentDictionary<Guid, Movie> _movies = new();
	private ConcurrentDictionary<Guid, Theater> _theaters = new();
	
	
	private BookMyShowService() {}
	public static BookMyShowService GetInstance()
	{
		if (_instance is null)
		{
			_instance = new BookMyShowService();
		}

		return _instance;
	}

	public Guid AddMovie(string movieName, string description, int runTime)
	{
		Movie movie = new (movieName, description, runTime);
		_movies.TryAdd(movie.Id, movie);
		return movie.Id;
	}

	public Guid AddTheater(string theaterName)
	{
		Theater theater = new (theaterName);
		_theaters.TryAdd(theater.Id, theater);
		return theater.Id;
	}

	public Screen AddScreen(Guid theaterId, int screenNumber)
	{
		_theaters.TryGetValue(theaterId, out var theater);
		if (theater is null)
		{
			return null;
		}
		
		Screen screen = new ($"Screen No: {screenNumber}");
		theater.AddScreen(screen);
		return screen;
	}

	public Show AddShowToScreen(Screen screen, Guid movieId)
	{
		_movies.TryGetValue(movieId, out var movie);
		if (movie is null)
		{
			return null;
		}
		Seat s1 = new("A1", SeatType.Gold, 150.12);
		Seat s2 = new("B1", SeatType.Silver, 100.23);
		Seat s3 = new("A2", SeatType.Gold, 150.12);
		Seat s4 = new("B2", SeatType.Silver, 100.23);
		Show show = screen.AddShow(new DateTime(2023, 10, 5, 14, 30, 0 ), new DateTime(2023, 10, 5, 17, 0, 0 ), movie);
		show.AddSeat(s1);
		show.AddSeat(s2);
		show.AddSeat(s3);
		show.AddSeat(s4);
		return show;
	}

	public void GetMovieDetails(Guid movieId)
	{
		_movies.TryGetValue(movieId, out var movie);
		if (movie is null)
		{
			return;
		}
		Console.WriteLine($"{movie.Name} : {movie.Description} : {movie.Duration}");
	}

	public void DisplayAllMovies(Guid theaterId)
	{
		_theaters.TryGetValue(theaterId, out var theater);
		if (theater is null)
		{
			return;
		}
		theater.Display();
	}
	
	public List<Screen> GetAllScreens(Guid theaterId)
	{
		_theaters.TryGetValue(theaterId, out var theater);
		if (theater is null)
		{
			return null;
		}

		return theater.GetAllScreen();
	}

	public List<Seat> GetAvailableSeats(Show show, SeatType type)
	{
		return show.GetAvailableSeatList(type);
	}

	public Booking Book(Guid userId, ImmutableList<Seat> selectedSeats)
	{
		Booking booking = new (userId, selectedSeats);
		booking.MakePayment();
		return booking;
	}
	
}

class BookMyShow
{
	public static void Main(string[] args)
	{
		User user = new("User Name", "name@email.com"); 
		BookMyShowService bookMyShowService = BookMyShowService.GetInstance();
		var movieId1 = bookMyShowService.AddMovie("Movie 1", "Description 1", 120);
		var movieId2 = bookMyShowService.AddMovie("Movie 2", "Description 2", 105);
		var theaterId = bookMyShowService.AddTheater("Theater 1");
		var screen1 = bookMyShowService.AddScreen(theaterId, 1);
		var screen2 = bookMyShowService.AddScreen(theaterId, 2);
		bookMyShowService.AddShowToScreen(screen1, movieId1);
		bookMyShowService.AddShowToScreen(screen1, movieId2);
		bookMyShowService.AddShowToScreen(screen2, movieId1);
		bookMyShowService.AddShowToScreen(screen2, movieId2);
		
		// Get Movie Details
		// bookMyShowService.GetMovieDetails(movieId1);
		
		// Display all Movies
		bookMyShowService.DisplayAllMovies(theaterId);
		
		// Get All screens
		var allScreens = bookMyShowService.GetAllScreens(theaterId);
		
		// Get All shows for a screen
		var selectedScreen = allScreens[0];
		var allShows = selectedScreen.GetAllShow();
		
		// Get All seats
		var seats = bookMyShowService.GetAvailableSeats(allShows[0], SeatType.Gold);
		foreach (var seat in seats)
		{
			Console.WriteLine($"{seat.SeatNo}");
		}
		
		// Book
		var selectedSeats = ImmutableList.Create<Seat>(seats[0]);
		var booking = bookMyShowService.Book(user.Id, selectedSeats);
		Console.WriteLine($"{booking.UserId} has booked Seats: {booking.SelectedSeats[0].SeatNo}");
		
		foreach (var seat in bookMyShowService.GetAvailableSeats(allShows[0], SeatType.Gold))
		{
			Console.WriteLine($"{seat.SeatNo}");
		}
	}
}