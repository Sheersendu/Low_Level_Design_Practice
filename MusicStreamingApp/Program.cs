class Song(string name, string artist, int duration)
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public string Name { get; set; } = name;
	public string Artist { get; set; } = artist;
	public int DurationInSeconds { get; set; } = duration;
}

class MusicPlayer
{
	private List<Song> _songList = new();
	private int _currentIndex = -1;
	private int _totalSongs;
	private Object _musicPlayer = new();
	private static Object _musicPlayerObject = new();
	private Song? currentSong;
	private bool IsPaused = false;
	private static MusicPlayer _instance; 
	
	private MusicPlayer() {}

	public static MusicPlayer GetInstance()
	{
		if (_instance == null)
		{
			lock (_musicPlayerObject)
			{
				if (_instance == null)
				{
					_instance = new MusicPlayer();
				}
			}
		}
		return _instance;
	}

	public void AddSong(Song newSong)
	{
		lock (_musicPlayer)
		{
			_songList.Add(newSong);
			_totalSongs += 1;
		}
	}

	public void GetPlaylist()
	{
		lock (_musicPlayer)
		{
			foreach (Song song in _songList)
			{
				Console.WriteLine($"Song : {song.Name} by {song.Artist}");
			}
		}
	}

	public List<Song> GetAllSongs()
	{
		lock (_musicPlayer)
		{
			return _songList.ToList();
		}
	}
	
	public void Play(int index = -1)
	{
		lock (_musicPlayer)
		{
			if (IsValidIndex(index))
			{
				_currentIndex = index;
				currentSong = _songList[_currentIndex];
				Console.WriteLine($"Playing {currentSong.Name} by {currentSong.Artist}");
			}
			else
			{
				Console.WriteLine("Select valid song!");
			}
		}
	}

	public void Pause()
	{
		lock (_musicPlayer)
		{
			Console.WriteLine($"Paused {currentSong?.Name} by {currentSong?.Artist}");
			IsPaused = true;
		}
	}

	public void Next()
	{
		lock (_musicPlayer)
		{
			_currentIndex += 1;
			_currentIndex %= _totalSongs;
			Play(_currentIndex);
		}
	}

	public void Previous()
	{
		lock (_musicPlayer)
		{
			_currentIndex -= 1;
			if (_currentIndex == -1)
			{
				_currentIndex = _totalSongs - 1;
			}
			Play(_currentIndex);
		}
	}

	private bool IsValidIndex(int index)
	{
		if (index >= 0 && index < _totalSongs)
		{
			return true;
		}

		return false;
	}
}

interface ISearchStrategy
{
	List<Song> Search(string searchTerm);
}

class SearchByName(MusicPlayer player) : ISearchStrategy
{
	private MusicPlayer _player = player;
	
	public List<Song> Search(string songName)
	{
		List<Song> allSongs = _player.GetAllSongs();
		return allSongs.Where(song => song.Name.Equals(songName, StringComparison.InvariantCulture)).ToList();
	}
}

class SearchByArtist(MusicPlayer player) : ISearchStrategy
{
	private MusicPlayer _player = player;
	
	public List<Song> Search(string artist)
	{
		List<Song> allSongs = _player.GetAllSongs();
		return allSongs.Where(song => song.Artist.Equals(artist, StringComparison.InvariantCulture)).ToList();
	}
}

enum DownloadType
{
	Http, Cloud
}

interface IDownloader
{
	void Download(Guid songId);
}

class HttpDownloader : IDownloader
{
	public void Download(Guid songId)
	{
		Console.WriteLine($"Downloading song : {songId} via HTTP.....");
	}
}

class CloudDownloader : IDownloader
{
	public void Download(Guid songId)
	{
		Console.WriteLine($"Downloading song : {songId} from cloud storage.....");
	}
}

class DownloaderFactory
{
	public static IDownloader GetDownloader(DownloadType type)
	{
		switch (type)
		{
			case DownloadType.Cloud:
				return new CloudDownloader();
			default:
				return new HttpDownloader();
		}
	}
}

class MusicPlayerDemo
{
	private MusicPlayer _player;
	private IDownloader _downloader;
	private ISearchStrategy _searchStrategy;
	
	public MusicPlayerDemo()
	{
		_player = MusicPlayer.GetInstance();
		_downloader = DownloaderFactory.GetDownloader(DownloadType.Http);
		_searchStrategy = new SearchByName(_player);
	}

	public Song AddSong(string songName, string artistName, int duration)
	{
		Song newSong = new (songName, artistName, duration);
		_player.AddSong(newSong);
		return newSong;
	}

	public void SetSearchStrategy(ISearchStrategy newStrategy)
	{
		_searchStrategy = newStrategy;
	}
	
	public void SetDownloader(IDownloader newDownloader)
	{
		_downloader = newDownloader;
	}

	public void GetPlaylist()
	{
		_player.GetPlaylist();
	}

	public void PlaySong(int songIndex)
	{
		_player.Play(songIndex);
	}
	
	public void PauseSong()
	{
		_player.Pause();
	}
	
	public void NextSong()
	{
		_player.Next();
	}
	
	public void PreviousSong()
	{
		_player.Previous();
	}

	public void Download(Guid songId)
	{
		_downloader.Download(songId);
	}

	public List<Song> Search(string searchTerm)
	{
		return _searchStrategy.Search(searchTerm);
	}
}


class MusicStreamingApp
{
	public static void Main(string[] args)
	{
		Console.WriteLine("Welcome to !Spotify");
		MusicPlayerDemo playerDemo = new();
		Song song1 = playerDemo.AddSong("Song 1", "Artist 1", 180);
		Song song2 = playerDemo.AddSong("Song 2", "Artist 2", 150);
		
		playerDemo.GetPlaylist();
		playerDemo.PlaySong(0);
		playerDemo.PauseSong();
		playerDemo.PlaySong(0);
		playerDemo.NextSong();
		playerDemo.PreviousSong();
		playerDemo.Download(song1.Id);
		foreach (Song song in playerDemo.Search("Song 1"))
		{
			Console.WriteLine($"{song.Name} - {song.Artist}");
		}
		
	}
}