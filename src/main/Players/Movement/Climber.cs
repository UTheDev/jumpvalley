﻿using Godot;

namespace Jumpvalley.Players.Movement
{
    /// <summary>
    /// Allows a character to climb objects.
    /// A MeshInstance3D can be climbed if it has a boolean metadata entry named "IsClimbable" that's set to true.
    /// </summary>
    public partial class Climber: Node
    {
        /// <summary>
        /// Name of the metadata entry that specifies whether or not a CollisionObject3D is climbable
        /// </summary>
        public readonly string IS_CLIMBABLE_METADATA_NAME = "IsClimbable";

        private RayCast3D rayCast;

        /// <summary>
        /// Whether or not the character is able to climb because the raycast has hit a climbable CollisionObject3D
        /// </summary>
        public bool CanClimb = false;

        /// <summary>
        /// The character's hitbox that this <see cref="Climber"/> is associated with.
        /// </summary>
        public CollisionShape3D Hitbox { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Climber"/>
        /// </summary>
        public Climber(CollisionShape3D hitbox)
        {
            Name = $"{nameof(Climber)}_{GetHashCode()}";

            Hitbox = hitbox;

            rayCast = new RayCast3D();
            rayCast.Name = $"{nameof(Climber)}_{GetHashCode()}_{nameof(rayCast)}";
            updateRayCast();
            hitbox.AddChild(rayCast);
        }

        private void updateRayCast()
        {
            BoxShape3D collisionBox = Hitbox.Shape as BoxShape3D;

            // For now, only box-shaped hitboxes will work.
            if (collisionBox == null) return;

            Vector3 boxSize = collisionBox.Size;

            // Start from center-height, then work our way to the bottom.
            // Remember that the position of the ray-cast is already relative to the hitbox's position
            // as it is parented to the hitbox itself
            rayCast.Position = new Vector3(0, 0, boxSize.Z / 2);
            rayCast.TargetPosition = new Vector3(0, -boxSize.Y / 2, 0);
        }
        
        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            CollisionObject3D collidedObject = rayCast.GetCollider() as CollisionObject3D;

            // If the collided object is a CollisionObject3D and it has a metadata entry
            // named "IsClimbable" set to true, we can climb.
            CanClimb = collidedObject != null
                && collidedObject.HasMeta(IS_CLIMBABLE_METADATA_NAME)
                && collidedObject.GetMeta(IS_CLIMBABLE_METADATA_NAME).AsBool() == true;

            //Vector3 boxPos = Hitbox.Position;

            // The raycast's position should be relative to the position and rotation of the character's hitbox
            //rayCast.Position = BoxPos + new Vector3(0, 0, BoxSize.Z / 2).Rotated(Vector3.Up, BoxRotation.Y);
            //rayCast.TargetPosition = new Vector3(0, BoxSize.Y / 2, 0);
        }
    }
}
