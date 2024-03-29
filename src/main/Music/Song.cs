using Godot;
using Jumpvalley.Audio;
using System.IO;

namespace Jumpvalley.Music
{
    /// <summary>
    /// Represents a single song associated with a file, along with some metadata
    /// </summary>
    public partial class Song: System.IDisposable
    {
        private bool _isLooping = false;

        /// <summary>
        /// Resource path of the stream that's requesting to be or currently open.
        /// <br/>
        /// This is kept track of to ensure that OpenStream() doesn't set the value of <see cref="Stream"/> to an AudioStream with an invalid resource path.
        /// </summary>
        private string streamResPath = null;

        public Song(string filePath, string name, string artists, string album)
        {
            FilePath = filePath;
            Name = name;
            Artists = artists;
            Album = album;
        }

        public Song() { }

        public Song(SongPackage package) : this(package.Path + "/" + package.SongPath, package.InfoFile.Name, package.InfoFile.Artists, package.InfoFile.Album) { }

        /// <summary>
        /// The actual audio stream that contains the sound data for playback by an <see cref="AudioStreamPlayer"/>
        /// </summary>
        public AudioStream Stream
        {
            get; private set;
        }

        /// <summary>
        /// Whether or not the song is looping
        /// </summary>
        public bool IsLooping
        {
            get => _isLooping;
            set
            {
                _isLooping = value;
                UpdateLoop();
            }
        }

        /// <summary>
        /// The file path to the song
        /// </summary>
        public string FilePath = null;

        /// <summary>
        /// The name of the song
        /// </summary>
        public string Name = null;

        /// <summary>
        /// The artists that made the song
        /// </summary>
        public string Artists = null;

        /// <summary>
        /// The album the song belongs to
        /// </summary>
        public string Album = null;

        // update function for IsLooping
        private void UpdateLoop()
        {
            if (Stream is AudioStreamWav sWav)
            {
                if (IsLooping)
                {
                    sWav.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
                }
                else
                {
                    sWav.LoopMode = AudioStreamWav.LoopModeEnum.Disabled;
                }
            }
            else if (Stream is AudioStreamOggVorbis sOgg)
            {
                sOgg.Loop = IsLooping;
            }
            else if (Stream is AudioStreamMP3 sMp3)
            {
                sMp3.Loop = IsLooping;
            }
        }

        /// <summary>
        /// Attempts to open up an AudioStream for the given <see cref="FilePath"/>
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the file under the given <see cref="FilePath"/> couldn't be found or when the file path is invalid.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// Thrown when the file was found, but the data format of the file is invalid.
        /// </exception>
        public void OpenStream()
        {
            if (Stream == null)
            {
                // This should be set before trying to load the corresponding stream so that once the stream is loaded, we can check if the
                // the stream has the correct resource path
                streamResPath = FilePath;

                // try loading the file
                AudioStreamReader audioStreamReader = new AudioStreamReader(streamResPath);
                AudioStream audioStream = audioStreamReader.Stream;

                if (streamResPath == null || !streamResPath.Equals(audioStream.ResourcePath))
                {
                    //audioStream.Free();
                    audioStream.Dispose();

                    return;
                }

                Stream = audioStream;
                UpdateLoop();

                /*
                Resource resource = GD.Load(streamResPath);

                if (resource == null)
                {
                    throw new FileNotFoundException("The file path of the song is invalid. Please make sure that such file path is correct and is an absolute file path.");
                }

                // update Stream variable
                if (resource is AudioStreamWav || resource is AudioStreamOggVorbis || resource is AudioStreamMP3)
                {
                    AudioStream audioStream = (AudioStream)resource;

                    // If the resource path of the loaded AudioStream doesn't match the file path, cancel the current operation.
                    // This can happen because CloseStream() was called while the resource was loading.
                    if (streamResPath == null || !streamResPath.Equals(audioStream.ResourcePath))
                    {
                        audioStream.Free();
                        audioStream.Dispose();

                        return;
                    }

                    Stream = audioStream;
                    UpdateLoop();
                }
                else
                {
                    throw new InvalidDataException("The data format of the song file is invalid. Please check that the song's file format is either WAV, OGG, or MP3.");
                }
                */
            }
        }

        /// <summary>
        /// Closes the AudioStream associated with this Song instance
        /// </summary>
        public void CloseStream()
        {
            streamResPath = null;

            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }
        }

        /// <summary>
        /// Constructs an attribution string in the following format:
        /// <br/>
        /// [song artist(s)] - [song name]
        /// </summary>
        /// <returns>The attribution string</returns>
        public string GetAttributionString()
        {
            return $"{Artists} - {Name}";
        }

        public void Dispose()
        {
            CloseStream();
        }
    }
}
