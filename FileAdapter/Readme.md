### Problem Statement: 
    Design a File Adapter Framework that converts one file type to another. Some adapters are already available, e.g., JSON → CSV, CSV → PDF. The goal was to create a generic framework that allows easy extension and file conversion.Leverage the existing adapters instead of creating input-output converter.

```
                 +--------------------+
                 |   IFileAdapter     |  <------ Interface for all adapters
                 +--------------------+
                 | + SourceFormat     |
                 | + DestinationFormat|
                 | + Convert(Stream)  |
                 +--------------------+
                         ▲
                         |
        +----------------+--------------------+
        |                |                    |
+----------------+ +----------------+ +----------------+
| JsonToCsvAdapter | | CsvToPdfAdapter | | JsonToExcelAdapter |
+----------------+ +----------------+ +----------------+
| Implements IFileAdapter for        |
| specific source/destination pairs |
+-----------------------------------+

            +--------------------+
            |  AdapterRegistry   |  <-- Singleton (or DI-injected)
            +--------------------+
            | Dictionary<(src,dst), IFileAdapter> |
            | + GetAdapter()                      |
            | + GetAdaptersForFileFormat()        |
            +--------------------+
                         ▲
                         |
             Injected into GenericAdapter
                         |
                 +------------------+
                 |  GenericAdapter  |  <-- Fallback adapter
                 +------------------+
                 | + BFS logic to   |
                 |   find conversion|
                 |   path if no     |
                 |   direct adapter |
                 +------------------+
                         ▲
                         |
                    Used by App

                           +------+
                           | App  |  <-- Orchestration Layer
                           +------+
                           | Gets adapter from registry
                           | Calls Convert() on adapter
                           +------+
```
Key Takeaways:

    IFileAdapter ensures plug-and-play adapter design.

    Adapters like JsonToCsvAdapter follow Single Responsibility.

    GenericAdapter enables indirect multi-step conversions via BFS.

    AdapterRegistry is the central registry (can be made a Singleton).

    Easily testable & future-proof — new adapters only need to implement the interface.