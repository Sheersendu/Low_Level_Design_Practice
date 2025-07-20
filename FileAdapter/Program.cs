namespace FileAdapter;

enum FileFormats
{
	JSON, CSV, PDF, Excel
}

interface IFileAdapter
{
	public FileFormats SourceFormat { get; set; }
	public FileFormats DestinationFormat { get; set; }
	public Stream Convert(Stream data);
}

class JsonToCsvAdapter : IFileAdapter
{
	public FileFormats SourceFormat { get; set; } = FileFormats.JSON;
	public FileFormats DestinationFormat { get; set; } = FileFormats.CSV;
	
	public Stream Convert(Stream data)
	{
		Console.WriteLine($"Converting {data} from Json to Csv!");
		return data;
	}
}

class JsonToExcelAdapter : IFileAdapter
{
	public FileFormats SourceFormat { get; set; } = FileFormats.JSON;
	public FileFormats DestinationFormat { get; set; } = FileFormats.Excel;
	
	public Stream Convert(Stream data)
	{
		Console.WriteLine($"Converting {data} from Json to Excel!");
		return data;
	}
}

class CsvToPdfAdapter : IFileAdapter
{
	public FileFormats SourceFormat { get; set; } = FileFormats.CSV;
	public FileFormats DestinationFormat { get; set; } = FileFormats.PDF;
	
	public Stream Convert(Stream data)
	{
		Console.WriteLine($"Converting {data} from Csv to PDf!");
		return data;
	}
}

class GenericAdapter : IFileAdapter
{
	public FileFormats SourceFormat { get; set; }
	public FileFormats DestinationFormat { get; set; }
	private AdapterRegistry _adapterRegistry;

	public GenericAdapter(FileFormats sourceFileFormat, FileFormats destinationFileFormat, AdapterRegistry adapterRegistry)
	{
		SourceFormat = sourceFileFormat;
		DestinationFormat = destinationFileFormat;
		_adapterRegistry = adapterRegistry;
	}

	private List<FileFormats> GetAdapterOrder()
	{
		Queue<(IFileAdapter, List<FileFormats> order)> adapterQueue = new();
		List<IFileAdapter> sourceAdapterList = _adapterRegistry.GetAdaptersForFileFormat(SourceFormat);
		List<FileFormats> adapterOrder = [SourceFormat];
		HashSet<IFileAdapter> visitedAdapters =  new ();
		foreach (IFileAdapter fileAdapter in sourceAdapterList)
		{
			adapterQueue.Enqueue((fileAdapter, adapterOrder));
			visitedAdapters.Add(fileAdapter);
		}

		while (adapterQueue.Count > 0)
		{
			var(intermediateFileAdapter, currentOrder)  = adapterQueue.Dequeue();
			if (intermediateFileAdapter.DestinationFormat == DestinationFormat)
			{
				return new List<FileFormats>(currentOrder){DestinationFormat};
			}
			foreach (IFileAdapter fileAdapter in _adapterRegistry.GetAdaptersForFileFormat(intermediateFileAdapter.DestinationFormat))
			{
				if (!visitedAdapters.Contains(fileAdapter))
				{
					visitedAdapters.Add(fileAdapter);
					adapterQueue.Enqueue((fileAdapter, new List<FileFormats>(currentOrder) { intermediateFileAdapter.DestinationFormat }));	
				}
			}
		}

		return [];
	}
	
	public Stream Convert(Stream data)
	{
		var adapterOrder = GetAdapterOrder();
		int adapterOrderLength = adapterOrder.Count;
		if (adapterOrderLength == 0)
		{
			Console.WriteLine($"Conversion not possible from {SourceFormat} to {DestinationFormat}!");
			return Stream.Null;
		}

		Stream convertedData = data;
		for (int index = 0; index < adapterOrderLength - 1; index++)
		{
			IFileAdapter adapter = _adapterRegistry.GetAdapter(adapterOrder[index], adapterOrder[index + 1]);
			convertedData = adapter.Convert(convertedData);
		}

		return convertedData;
	}
}

class AdapterRegistry
{
	private Dictionary<(FileFormats source, FileFormats destination), IFileAdapter> adapters;

	public AdapterRegistry()
	{
		adapters = new();
		IFileAdapter csvToPdfAdapter = new CsvToPdfAdapter();
		IFileAdapter jsonToCsvAdapter = new JsonToCsvAdapter();
		IFileAdapter jsonToExcelAdapter = new JsonToExcelAdapter();
		adapters.Add((FileFormats.CSV, FileFormats.PDF), csvToPdfAdapter);
		adapters.Add((FileFormats.JSON, FileFormats.CSV), jsonToCsvAdapter);
		adapters.Add((FileFormats.JSON, FileFormats.Excel), jsonToExcelAdapter);
	}

	public IFileAdapter GetAdapter(FileFormats source, FileFormats destination)
	{
		return adapters.GetValueOrDefault((source, destination), new GenericAdapter(source, destination, this));
	}

	public List<IFileAdapter> GetAdaptersForFileFormat(FileFormats format)
	{
		return adapters.Where(key => key.Key.source == format).Select(key => key.Value).ToList();
	}
}

class FileAdapter
{
	public static void Main(string[] args)
	{
		AdapterRegistry adapterRegistry = new ();
		byte[] byteArray = "Hello World!"u8.ToArray();
		Stream data = new MemoryStream(byteArray);
		
		// IFileAdapter csvToPdfAdapter = adapterRegistry.GetAdapter(FileFormats.CSV, FileFormats.PDF);
		// csvToPdfAdapter.Convert(data);
		// IFileAdapter jsonToExcelAdapter = adapterRegistry.GetAdapter(FileFormats.JSON, FileFormats.Excel);
		// jsonToExcelAdapter.Convert(data);
		IFileAdapter adapter1 = adapterRegistry.GetAdapter(FileFormats.JSON, FileFormats.PDF);
		adapter1.Convert(data);
		IFileAdapter adapter2 = adapterRegistry.GetAdapter(FileFormats.Excel, FileFormats.PDF);
		adapter2.Convert(data);
	}
}
