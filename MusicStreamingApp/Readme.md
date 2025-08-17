
### Design a music player with search, play/pause/next/previous, and song download features in 30–40 minutes.

| Feature                                                   | Pattern                               | Why                                                                                                                                 |
| --------------------------------------------------------- | ------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| **Play / Pause / Next / Previous**                        | **Command**                           | Encapsulates playback actions, makes it easy to queue, undo, or bind to UI buttons.                                                 |
| **Central Music Player state**                            | **Singleton**                         | Only one active player at a time controlling state (current song, elapsed time, playlist).                                          |
| **Search Music**                                          | **Strategy**                          | Can plug in different search algorithms (local search, cloud API search, AI-powered search) without changing the search caller.     |
| **Download Songs**                                        | **Factory Method / Abstract Factory** | Create the right downloader object based on song source (MP3, FLAC, streaming link, cloud bucket). This keeps creation logic clean. |
| **Adding extra features (lyrics, equalizer, visualizer)** | **Decorator**                         | Wrap a base player with extra capabilities without modifying core player code.                                                      |
| **Notification to UI (song change, download complete)**   | **Observer**                          | UI can subscribe to events from the player without tight coupling.                                                                  |
