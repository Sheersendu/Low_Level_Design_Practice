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
			IsAvailable = false;
		}
	}
}

class Screen(string name, Movie movie) : BaseClass
{
	public string Name { get; set; } = name;
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
		Console.WriteLine($"---------------{Name}---------------");
		foreach (var screen in _screenList.Values)
		{
			Console.WriteLine($"Now '{screen.Movie.Name}' in cinemas at '{screen.Name}'");
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
	private string BookingId { get; init; } = new Random().Next().ToString();
	private readonly ImmutableList<Seat> _selectedSeats;
	private readonly Payment _payment = new ();
	private double _billAmount = 0;

	public Booking(ImmutableList<Seat> selectedSeats)
	{
		_payment.SetStatus(PaymentStatus.Success);
		_selectedSeats = selectedSeats;
	}

	private void CalculateTotalAmount()
	{
		foreach (var seat in _selectedSeats)
		{
			seat.Book();
			_billAmount += seat.Price;
		}

		_billAmount = Math.Round(_billAmount, 2);
	}
	
	public void GetPaymentStatus()
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

	public void AddScreen(Guid theaterId, Guid movieId)
	{
		_movies.TryGetValue(movieId, out var movie);
		_theaters.TryGetValue(theaterId, out var theater);
		if (movie is null || theater is null)
		{
			return;
		}
		Seat s1 = new("A1", SeatType.Gold, 150.12);
		Seat s2 = new("B1", SeatType.Silver, 100.23);
		Seat s3 = new("A2", SeatType.Gold, 150.12);
		Seat s4 = new("B2", SeatType.Silver, 100.23);
		Screen screen = new ($"Screen No: {new Random().Next()%10+1}", movie);
		screen.AddSeat(s1);
		screen.AddSeat(s2);
		screen.AddSeat(s3);
		screen.AddSeat(s4);
		theater.AddScreen(screen);
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

	public List<Seat> GetAvailableSeats(Screen screen, SeatType type)
	{
		return screen.GetAvailableSeatList(type);
	}

	public Booking Book(ImmutableList<Seat> selectedSeats)
	{
		Booking booking = new (selectedSeats);
		return booking;
	}
	
}

class BookMyShow
{
	public static void Main(string[] args)
	{
		// Movie movie1 = new("Movie 1", "Description 1", 120);
		// Movie movie2 = new("Movie 2", "Description 2", 105);
		// Seat s1 = new("A1", SeatType.Gold, 150.12);
		// Seat s2 = new("B1", SeatType.Silver, 100.23);
		// Seat s3 = new("A2", SeatType.Gold, 150.12);
		// Screen screen1 = new Screen("Screen 1", movie1);
		// screen1.AddSeat(s1);
		// screen1.AddSeat(s2);
		// screen1.AddSeat(s3);
		// Screen screen2 = new Screen("Screen 2", movie2);
		// screen2.AddSeat(s1);
		// screen2.AddSeat(s2);
		// screen2.AddSeat(s3);
		
		#region Remove Seat

		// screen.RemoveSeat(s3.Id);
		// foreach (var seat in screen.GetAvailableSeatList(SeatType.Gold))
		// {
		// 	Console.WriteLine($"{seat.SeatNo} - {seat.Type} - {seat.Price} - {seat.IsAvailable}");
		// }

		#endregion

		#region Check booking

		// s1.Book();
		// foreach (var seat in screen.GetAvailableSeatList(SeatType.Gold))
		// {
		// 	Console.WriteLine($"{seat.SeatNo} - {seat.Type} - {seat.Price} - {seat.IsAvailable}");
		// }

		#endregion

		#region Check Get Seat

		// Console.WriteLine(screen.GetSeat(s1.Id).SeatNo);

		#endregion

		// Theater theater = new Theater("Theater 1");
		// theater.AddScreen(screen1);
		// theater.AddScreen(screen2);
		// theater.Display();
		
		// foreach (var seat in screen1.GetAvailableSeatList(SeatType.Gold))
		// {
		// 	Console.WriteLine($"{seat.SeatNo} - {seat.Type} - {seat.Price} - {seat.IsAvailable}");
		// }
		// var selectedSeats = ImmutableList.Create<Seat>(s1, s2);
		// Booking booking = new Booking(selectedSeats);
		// booking.GetPaymentStatus();

		BookMyShowService bookMyShowService = BookMyShowService.GetInstance();
		var movieId1 = bookMyShowService.AddMovie("Movie 1", "Description 1", 120);
		var movieId2 = bookMyShowService.AddMovie("Movie 2", "Description 2", 105);
		var theaterId = bookMyShowService.AddTheater("Theater 1");
		bookMyShowService.AddScreen(theaterId, movieId1);
		bookMyShowService.AddScreen(theaterId, movieId2);
		
		// Get Movie Details
		// bookMyShowService.GetMovieDetails(movieId1);
		
		// Display all Movies
		// bookMyShowService.DisplayAllMovies(theaterId);
		
		// Get All screens
		var allScreens = bookMyShowService.GetAllScreens(theaterId);
		
		// Get All seats
		var seats = bookMyShowService.GetAvailableSeats(allScreens[0], SeatType.Gold);
		foreach (var seat in seats)
		{
			Console.WriteLine($"{seat.SeatNo}");
		}
		
		// Book
		var selectedSeats = ImmutableList.Create<Seat>(seats[0]);
		bookMyShowService.Book(selectedSeats);
		
		foreach (var seat in bookMyShowService.GetAvailableSeats(allScreens[0], SeatType.Gold))
		{
			Console.WriteLine($"{seat.SeatNo}");
		}
	}
}