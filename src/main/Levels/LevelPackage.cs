﻿using Godot;
using Jumpvalley.IO;
using System;

namespace Jumpvalley.Levels
{
    /// <summary>
    /// This class represents a level package, a filesystem folder that contains the components of a level.
    /// <br/>
    /// Each level package includes:
    /// <list type="bullet">
    /// <item>The level's info file</item>
    /// <item>The Godot package (PCK file) that contains the level's contents</item>
    /// </list>
    /// </summary>
    public partial class LevelPackage : IDisposable
    {
        /// <summary>
        /// Status codes that indicate the result of an attempt to load a level's resource pack
        /// </summary>
        public enum ResourcePackLoadStatus
        {
            /// <summary>
            /// Indicates that the Godot package was loaded successfully.
            /// </summary>
            Success = 0,

            /// <summary>
            /// Indicates that the Godot package under the given file name could not be found.
            /// </summary>
            FileNotFound = 1,

            /// <summary>
            /// Indicates that the Godot package under the given file name was found, but loading it failed.
            /// </summary>
            LoadingFailed = 2
        }

        /// <summary>
        /// The level's info file
        /// </summary>
        public LevelInfoFile Info { get; private set; }

        /// <summary>
        /// The directory path of the level package
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// If the level's root node has been loaded, this will point to it.
        /// </summary>
        public Node RootNode { get; private set; }

        /// <summary>
        /// The level instance associated with this level
        /// </summary>
        public Level LevelInstance { get; private set; }

        /// <summary>
        /// Constructs a LevelPackage object for a given level package's directory
        /// </summary>
        /// <param name="path">The directory path of the level package</param>
        public LevelPackage(string path)
        {
            Path = path;

            using FileAccess infoFile = FileAccess.Open($"{path}/{InfoFile.FILE_NAME}", FileAccess.ModeFlags.Read);
            if (infoFile == null)
            {
                throw new Exception($"Failed to open the corresponding {InfoFile.FILE_NAME} file. This is the message returned by FileAccess.GetOpenError(): {FileAccess.GetOpenError()}");
            }

            Info = new LevelInfoFile(infoFile.GetAsText());
        }

        /// <summary>
        /// Attempts to load the resource pack specified within the level's info file if it exists
        /// </summary>
        /// <returns>
        /// A status code indicating the results of the attempt to load the resource pack
        /// </returns>
        public ResourcePackLoadStatus TryLoadResourcePack()
        {
            string resourcePackPath = $"{Path}/{Info.ResourcePackPath}";
            if (!FileAccess.FileExists(resourcePackPath))
            {
                return ResourcePackLoadStatus.FileNotFound;
            }

            // LoadResourcePack will also only return true if the loading of the resource pack succeeded
            if (ProjectSettings.LoadResourcePack(resourcePackPath, false))
            {
                return ResourcePackLoadStatus.Success;
            }

            return ResourcePackLoadStatus.LoadingFailed;
        }

        /// <summary>
        /// Attempts to load the level's main scene.
        /// If this operation is successful, <see cref="RootNode"/> will be set to the scene's root node, and this method will return true.
        /// Otherwise, this method will return false.
        /// </summary>
        public bool LoadRootNode()
        {
            if (RootNode != null) throw new InvalidOperationException("There's already a root node loaded. Please unload it first.");

            PackedScene packedScene = GD.Load<PackedScene>($"res://levels/{Info.Id}/{Info.ScenePath}");

            if (packedScene == null)
            {
                // TO-DO: return status codes for errors regarding the loading of the level's scene
                return false;
            }

            RootNode = packedScene.Instantiate();
            packedScene.Dispose();

            return true;
        }

        /// <summary>
        /// Unloads and disposes of the level's root node.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when LevelInstance isn't null.
        /// This is to prevent errors that can occur when the root node is disposed while the level is still active.
        /// </exception>
        public void DisposeRootNode()
        {
            if (LevelInstance != null) throw new InvalidOperationException("Root node cannot be disposed while LevelInstance is not null.");

            if (RootNode != null)
            {
                RootNode.QueueFree();
                RootNode.Dispose();
                RootNode = null;
            }
        }

        /// <summary>
        /// Creates a level instance for the current <see cref="RootNode"/> and assigns it to <see cref="LevelInstance"/>
        /// </summary>
        public void CreateLevelInstance()
        {
            if (LevelInstance == null && RootNode != null)
            {
                LevelInstance = new Level(Info, RootNode);
            }
        }

        /// <summary>
        /// Disposes of <see cref="LevelInstance"/> if it's not null.
        /// </summary>
        public void DisposeLevelInstance()
        {
            if (LevelInstance != null)
            {
                LevelInstance.Dispose();
                LevelInstance = null;
            }
        }

        /// <summary>
        /// Initializes the level assigned to <see cref="LevelInstance"/> (if that hasn't been done yet), and then starts it.
        /// </summary>
        public void StartLevel()
        {
            if (LevelInstance != null)
            {
                if (!LevelInstance.IsInitialized)
                {
                    LevelInstance.Initialize();
                }

                LevelInstance.Start();
            }
        }

        /// <summary>
        /// Stops the level assigned to <see cref="LevelInstance"/>
        /// </summary>
        public void StopLevel()
        {
            LevelInstance?.Stop();
        }

        /// <summary>
        /// Disposes of this level package, including the level instance associated with it.
        /// </summary>
        public void Dispose()
        {
            StopLevel();
            DisposeLevelInstance();
            DisposeRootNode();
        }
    }
}
