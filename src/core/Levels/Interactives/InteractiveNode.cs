﻿using Godot;
using System;

using Jumpvalley.Timing;

namespace Jumpvalley.Levels.Interactives
{
    /// <summary>
    /// A subclass of <see cref="Interactive"/> that operates over a Godot node.
    /// It makes using a Godot node's properties and metadata easier.
    /// See the relevant <see href="https://github.com/UTheCat/jumpvalley/wiki/Interactives#node-based-interactives">wiki section</see> for details.
    /// </summary>
    public partial class InteractiveNode : Interactive
    {
        /// <summary>
        /// What an interactive's node marker name should begin with
        /// </summary>
        public static readonly string NODE_MARKER_NAME_PREFIX = "_Interactive";

        /// <summary>
        /// The node which indicates that its parent node is the root node of an interactive.
        /// Its name should begin with <c>_Interactive</c>
        /// </summary>
        public Node NodeMarker { get; private set; }

        /// <summary>
        /// The interactive's root node
        /// </summary>
        public Node RootNode => NodeMarker.GetParent();

        /// <summary>
        /// Creates a new instance of <see cref="InteractiveNode"/> for a given <see cref="Stopwatch"/> and <see cref="Node"/>
        /// </summary>
        /// <param name="stopwatch">The stopwatch to bind the interactive to</param>
        /// <param name="node">The node to operate over</param>
        public InteractiveNode(OffsetStopwatch stopwatch, Node nodeMarker) : base(stopwatch)
        {
            if (nodeMarker == null) throw new ArgumentNullException(nameof(nodeMarker));

            NodeMarker = nodeMarker;
        }

        /// <summary>
        /// Attempts to get the value of a metadata entry with a specified name
        /// if it exists and assigns it to the <paramref name="meta"/> reference variable.
        /// </summary>
        /// <param name="name">The name of the metadata entry</param>
        /// <param name="meta">The reference variable where the metadata entry value should be stored</param>
        /// <returns>
        /// <c>true</c> if the metadata entry under the given name was found
        /// and grabbing its value was successful, <c>false</c> otherwise.
        /// </returns>
        public bool TryGetMeta(string name, out Variant meta)
        {
            Node marker = NodeMarker;
            if (marker != null)
            {
                if (marker.HasMeta(name))
                {
                    meta = marker.GetMeta(name);
                    return true;
                }
            }

            meta = default;
            return false;
        }

        /// <summary>
        /// Version of <see cref="TryGetMeta(string, out Variant)"/>
        /// that outputs the metadata entry value as a specified type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <param name="name">The name of the metadata entry</param>
        /// <param name="meta">The reference variable where the metadata entry value should be stored</param>
        /// <returns>
        /// <c>true</c> if the metadata entry under the given name was found
        /// and grabbing its value was successful, <c>false</c> otherwise.
        /// </returns>
        public bool TryGetMeta<[MustBeVariant] T>(string name, out T meta)
        {
            Variant metadataValue;
            bool success = TryGetMeta(name, out metadataValue);

            if (success)
            {
                meta = metadataValue.As<T>();
                return true;
            }

            meta = default;
            return false;
        }

        /// <summary>
        /// Sets the metadata entry belonging to <see cref="NodeMarker"/>
        /// with the given name to the specified value.
        /// </summary>
        /// <param name="name">The name of the metadata entry</param>
        /// <param name="value">The value to set the metadata entry to</param>
        public void SetMeta(string name, Variant value)
        {
            NodeMarker.SetMeta(name, value);
        }

        /// <summary>
        /// Event that's raised when one of the metadata of the node changes.
        /// </summary>
        //public event EventHandler<NodeMetadataChangedArgs> NodeMetadataChanged;
    }
}
