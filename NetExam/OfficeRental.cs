namespace NetExam
{
    using System;
    using System.Collections.Generic;
    using NetExam.Abstractions;
    using NetExam.Dto;
    using System.Linq;
    using NetExam.Models;

    public class OfficeRental : IOfficeRental
    {
        List<Location> Locations = new List<Location>();
        List<Office> Offices = new List<Office>();
        List<Booking> Bookings = new List<Booking>();

        public void AddLocation(LocationSpecs locationSpecs)
        {
            if (CheckIfLocationisDuplicate(locationSpecs.Name)) throw new Exception($"Already existing {locationSpecs.Name}.");
            this.Locations.Add(new Location(locationSpecs.Name, locationSpecs.Neighborhood));
        }

        public void AddOffice(OfficeSpecs officeSpecs)
        {
            if (!CheckIfLocationisDuplicate(officeSpecs.LocationName)) throw new Exception($"Non-existing {officeSpecs.LocationName}.");
            if (CheckIfOfficeisDuplicate(officeSpecs.LocationName)) throw new Exception($"{officeSpecs.LocationName} already added.");
            this.Offices.Add(new Office(officeSpecs.LocationName, officeSpecs.Name, officeSpecs.MaxCapacity, officeSpecs.AvailableResources));
        }

        public void BookOffice(BookingRequest bookingRequest)
        {
            if (CheckIfBookingisTaken(bookingRequest.OfficeName)) throw new Exception($"{bookingRequest.OfficeName} is already in use.");
            if (CheckIfBookingisDuplicate(bookingRequest.UserName)) throw new Exception($"You've already booked {bookingRequest.OfficeName}.");
            this.Bookings.Add(new Booking(bookingRequest.LocationName, bookingRequest.OfficeName, bookingRequest.DateTime, bookingRequest.Hours, bookingRequest.UserName));
        }

        public IEnumerable<IBooking> GetBookings(string locationName, string officeName)
        {
            return this.Bookings.FindAll(x => x.LocationName == locationName && x.OfficeName == officeName);
        }

        public IEnumerable<ILocation> GetLocations()
        {
            return this.Locations;
        }

        public IEnumerable<IOffice> GetOffices(string locationName)
        {
            return this.Offices.FindAll(x => x.LocationName == locationName);
        }

        public IEnumerable<IOffice> GetOfficeSuggestion(SuggestionRequest suggestionRequest)
        {
            var idealOffices = Offices.Where
                (o => 
                    o.MaxCapacity >= suggestionRequest.CapacityNeeded &&
                    o.AvailableResources.Intersect(suggestionRequest.ResourcesNeeded).Count() == suggestionRequest.ResourcesNeeded.Count() &&
                    suggestionRequest.PreferedNeigborHood == Locations.Single(l => l.Name == o.LocationName).Neighborhood &&
                    !CheckIfOfficeisTaken(o.Name))
                        .OrderBy(o => o.MaxCapacity)
                        .ThenBy(o => o.AvailableResources.Count());

            var suggestedOffices = Offices.Where
                (o => o.MaxCapacity >= suggestionRequest.CapacityNeeded &&
                    o.AvailableResources.Intersect(suggestionRequest.ResourcesNeeded).Count() >= suggestionRequest.ResourcesNeeded.Count() &&
                    suggestionRequest.PreferedNeigborHood != Locations.Single(l => l.Name == o.LocationName).Neighborhood &&
                    !CheckIfOfficeisTaken(o.Name))
                        .OrderBy(o => o.MaxCapacity)
                        .ThenBy(o => o.AvailableResources.Count());

            var result = idealOffices.Union(suggestedOffices);

            return result;
        }

        private bool CheckIfLocationisDuplicate(string locationName)
        {
            return this.Locations.Find((location) => location.Name == locationName) != null;
        }
        
        private bool CheckIfOfficeisDuplicate(string officeName)
        {
            return this.Offices.Find((office) => office.Name == officeName) != null;
        }

        private bool CheckIfBookingisDuplicate(string username)
        {
            return this.Bookings.Find((booking) => booking.UserName == username) != null;
        }

        private bool CheckIfBookingisTaken(string officeName)
        {
            return this.Bookings.Find((booking) => booking.OfficeName == officeName) != null;
        }
        
        private bool CheckIfOfficeisTaken(string officeName)
        {
            return this.Bookings.Find((booking) => booking.OfficeName == officeName) != null;
        }
    }
}