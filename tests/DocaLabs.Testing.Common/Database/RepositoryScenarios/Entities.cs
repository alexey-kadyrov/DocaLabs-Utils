using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DocaLabs.Storage.Core;

namespace DocaLabs.Testing.Common.Database.RepositoryScenarios
{
    public class Tile : IEntity
    {
        public virtual Guid Id { get; set; }

        [ConcurrencyCheck]
        public virtual byte[] Version { get; set; }

        public virtual IList<InterestingPoint> Points { get; set; }

        public virtual IList<Place> Places { get; set; }
    }

    public class InterestingPoint : IEntity
    {
        public virtual Guid Id { get; set; }

        public virtual string Category { get; set; }

        [ConcurrencyCheck]
        public virtual byte[] Version { get; set; }

        public virtual Tile Tile { get; set; }

        public virtual IList<Place> Places { get; set; }
    }

    public class Place : IEntity
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        [ConcurrencyCheck]
        public virtual byte[] Version { get; set; }

        public virtual Tile Tile { get; set; }

        public virtual IList<InterestingPoint> Points { get; set; }
    }
}
