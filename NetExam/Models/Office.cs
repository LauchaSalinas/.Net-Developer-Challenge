using System;
using System.Collections.Generic;
using System.Text;
using NetExam.Abstractions;

namespace NetExam.Models
{
    internal class Office : IOffice
    {
        public string LocationName { get; }
        public string Name { get; }
        public int MaxCapacity { get; set; }
        public IEnumerable<string> AvailableResources { get; }

        public Office(string locationName, string name, int maxCapacity, IEnumerable<string> availableResources = null)
        {
            if (string.IsNullOrWhiteSpace(locationName)) throw new ArgumentException(nameof(locationName));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            if (maxCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(maxCapacity));

            LocationName = locationName;
            Name = name;
            MaxCapacity = maxCapacity;
            AvailableResources = availableResources ?? new List<string>();
        }
    }
}
