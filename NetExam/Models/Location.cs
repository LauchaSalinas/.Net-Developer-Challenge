using System;
using System.Collections.Generic;
using System.Text;
using NetExam.Abstractions;

namespace NetExam.Models
{
    internal class Location : ILocation
    {
        public string Name { get ; }
        public string Neighborhood { get; }

        public Location(string name, string neighborhood)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            if (string.IsNullOrWhiteSpace(neighborhood)) throw new ArgumentException(nameof(neighborhood));

            Name = name;
            Neighborhood = neighborhood;
        }
    }
}
