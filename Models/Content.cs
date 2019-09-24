using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FeeforFreedom.Models
{
    public abstract class Content
    {
        public virtual string Name { get; set; }
        public virtual string ReleaseDate { get; set; }
        public virtual string Category { get; set; }
        public virtual string Writer { get; set; }
        public string link { get; set; }
        public string bannerLink { get; set; }

    }
    /// <summary>
    ///Empty class created for defining to pseudo delete key in database.
    /// </summary>
    internal class PseudoDeleteAttribute : Attribute//uses for marked delete attribute
    {
    }
}
