﻿using System;

namespace NSwag.Annotations
{
    /// <summary>Title of the entity for x-ms-summary. Example - 'Task Name', 'Due Date', etc. It is recommended that you use title case.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class FlowTitleAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="FlowTitleAttribute"/> class.</summary>
        /// <param name="description">The description.</param>
        public FlowTitleAttribute(string description)
        {
            Description = description;
        }

        /// <summary>Gets or sets the description.</summary>
        public string Description { get; set; }
    }
}