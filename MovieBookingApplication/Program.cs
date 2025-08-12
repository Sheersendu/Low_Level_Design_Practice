using System.Collections.Concurrent;

class BaseClass
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

class City(string name) : BaseClass
{
	public string Name { get; set; } = name;
}

class Movie(string name, string description, int duration) : BaseClass
{
	public string Name { get; set; } = name;
	public string Description { get; set; } = description;
	public int Duration { get; set; } = duration;
}

enum SeatType
{
	Gold , Silver
}

enum SeatStatus
{
	Available, Booked, Locked
}

static class SeatTypeExtension
{
	public static double GetPrice(this SeatType type)
	{
		switch (type)
		{
			case SeatType.Gold:
				return 100.50;
			case SeatType.Silver:
				return 75.99;
			default:
				return 50.25;
		}
	}
}

class Seat(string number, SeatType seatType) : BaseClass
{
	public string Number { get; set; } = number;
	public SeatType Type { get; set; } = seatType;
	public SeatStatus Status { get; set; } = SeatStatus.Available;
	private object _seatLock = new();

	public void Reserve()
	{
		lock (_seatLock)
		{
			Status = SeatStatus.Locked;
		}
	}

	public void Book()
	{
		lock (_seatLock)
		{
			Status = SeatStatus.Booked;
		}	
	}
	
	public void UnReserve()
	{
		lock (_seatLock)
		{
			Status = SeatStatus.Available;
		}
	}
}

class Show(Movie currentMovie, DateTime startTime) : BaseClass
{
	public Movie movie { get; set; } = currentMovie;
	public DateTime StartTime { get; set; } = startTime;
	private ConcurrentDictionary<Guid, Seat> _seats = new();
	private object _showLock = new();

	public void AddSeat(Seat newSeat)
	{
		_seats.TryAdd(newSeat.Id, newSeat);
	}
	
	public void RemoveSeat(Guid seatId)
	{
		_seats.TryRemove(seatId, out _);
	}

	public List<Seat> GetAvailableSeats()
	{
		lock (_showLock)
		{
			return _seats.Values.Where(seat => seat.Status == SeatStatus.Available).ToList();
		}
	}
}

class Screen(string name) : BaseClass
{
	public string Name { get; set; } = name;
	private readonly ConcurrentDictionary<Guid, Show> _shows = new();
	
	public void AddShow(Movie currentMovie, DateTime startTime)
	{
		Show newShow = new(currentMovie, startTime);
		_shows.TryAdd(newShow.Id, newShow);
	}
	
	public void RemoveShow(Guid showId)
	{
		_shows.TryRemove(showId, out _);
	}

	public List<Show> GetShowsByMovie(string movieName)
	{
		return _shows.Values.Where(show => show.movie.Name.Equals(movieName, StringComparison.InvariantCulture)).OrderBy(show => show.StartTime).ToList();
	}
	
}

class Theater(string name, City currentCity) : BaseClass
{
	public string Name { get; set; } = name;
	public City city { get; set; } = currentCity;
	private readonly ConcurrentDictionary<Guid, Screen> _screens = new();

	public void AddScreen(Screen screen)
	{
		_screens.TryAdd(screen.Id, screen);
	}
	
	public void RemoveScreen(Screen screen)
	{
		_screens.TryRemove(screen.Id, out _);
	}

	public List<Show> FindShowsByMovie(string movieName)
	{
		var allShows = new List<Show>();
		foreach (Screen screen in _screens.Values)
		{
			var availableShows = screen.GetShowsByMovie(movieName);
			allShows.AddRange(availableShows);
		}

		return allShows;
	}
	
}

interface IPricingStrategy
{
	double CalculatePrice(List<Seat> seats);
}

class WeekdayPricing : IPricingStrategy
{
	public double CalculatePrice(List<Seat> seats)
	{
		double totalPrice = 0;
		foreach (Seat seat in seats)
		{
			totalPrice += seat.Type.GetPrice();
		}

		return totalPrice;
	}
}

class WeekEndPricing : IPricingStrategy
{
	private const double SurgeRate = 1.5; 
		
	public double CalculatePrice(List<Seat> seats)
	{
		double totalPrice = 0;
		foreach (Seat seat in seats)
		{
			totalPrice += seat.Type.GetPrice();
		}

		return totalPrice*SurgeRate;
	}
}

class MovieBookingManager
{
	private static MovieBookingManager _instance;
	private static object _movieManager = new();
	public IPricingStrategy PricingStrategy = new WeekdayPricing();
	private ConcurrentDictionary<Guid, Theater> _theaters = new();
	
	private MovieBookingManager(){}

	public static MovieBookingManager GetInstance()
	{
		if (_instance is null)
		{
			lock (_movieManager)
			{
				if (_instance is null)
				{
					_instance = new MovieBookingManager();
				}
			}
		}
		return _instance;
	}

	public void SetStrategy(IPricingStrategy pricingStrategy)
	{
		PricingStrategy = pricingStrategy;
	}

	public double GetPrice(List<Seat> selectedSeats)
	{
		return PricingStrategy.CalculatePrice(selectedSeats);
	}

	public void AddTheater(string theaterName, City currentCity)
	{
		Theater theater = new(theaterName, currentCity);
		_theaters.TryAdd(theater.Id, theater);
	}

	public Theater? GetTheater(Guid theaterId)
	{
		if (_theaters.TryGetValue(theaterId, out var theater))
		{
			return theater;
		}

		Console.WriteLine("No theater found!");
		return null;
	}

	public List<Theater> GetTheaterByCity(string cityName)
	{
		return _theaters.Values.Where(theater => theater.city.Name.Equals(cityName)).ToList();
	}

	public void BookSeats(List<Seat> bookedSeats)
	{
		foreach (var seat in bookedSeats)
		{
			seat.Book();
		}
	}
	
	public void ConfirmSeats(List<Seat> confirmedSeats)
	{
		foreach (var seat in confirmedSeats)
		{
			seat.Reserve();
		}
	}
}

class MovieBookingDemo
{
	public static void Main(string[] args)
	{
		MovieBookingManager manager = MovieBookingManager.GetInstance();
		City Bangalore = new("Bangalore");
		Movie movie = new ("Movie 1", "Description", 120);
		var theaterList = manager.GetTheaterByCity("Bangalore");
		var theater = manager.GetTheater(theaterList[0].Id);
		var showList = theater.FindShowsByMovie("Movie 1");
		var selectedShow = showList[0];
		var allSeats = selectedShow.GetAvailableSeats();
		var selectedSeat = new List<Seat> { allSeats[0], allSeats[1] };
		manager.BookSeats(selectedSeat);
		manager.ConfirmSeats(selectedSeat);
		Console.WriteLine($"{manager.GetPrice(selectedSeat)}");
	}
}