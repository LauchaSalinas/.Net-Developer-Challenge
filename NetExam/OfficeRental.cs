namespace NetExam
{
    using System;
    using System.Collections.Generic;
    using NetExam.Abstractions;
    using NetExam.Dto;
    using System.Linq;

    public class OfficeRental : IOfficeRental
    {
        List<LocationSpecs> Locations = new List<LocationSpecs>();
        List<OfficeSpecs> Offices = new List<OfficeSpecs>();
        List<BookingRequest> Bookings = new List<BookingRequest>();

        public void AddLocation(LocationSpecs locationSpecs)
        {
            if (CheckIfLocationisDuplicate(locationSpecs.Name)) throw new Exception($"Already existing {locationSpecs.Name}.");
            this.Locations.Add(locationSpecs);
        }

        public void AddOffice(OfficeSpecs officeSpecs)
        {
            if (!CheckIfLocationisDuplicate(officeSpecs.LocationName)) throw new Exception($"Non-existing {officeSpecs.LocationName}.");
            if (CheckIfOfficeisDuplicate(officeSpecs.LocationName)) throw new Exception($"{officeSpecs.LocationName} already added.");
            this.Offices.Add(officeSpecs);
        }

        public void BookOffice(BookingRequest bookingRequest)
        {
            if (CheckIfBookingisTaken(bookingRequest.OfficeName)) throw new Exception($"{bookingRequest.OfficeName} is already in use.");
            if (CheckIfBookingisDuplicate(bookingRequest.UserName)) throw new Exception($"You've already booked {bookingRequest.OfficeName}.");
            this.Bookings.Add(bookingRequest);
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
            IEnumerable<IOffice> SizeCapableOffices = GetSizeCapableOffices(suggestionRequest);
            IEnumerable<IOffice> PreferedNeighboorhoodOffices = GetPreferedNeighboorhoodOffices(suggestionRequest);
            IEnumerable<IOffice> SuitableResourcesOffices = GetSuitableResourcesOffices(suggestionRequest);
            IEnumerable<IOffice> AvailableOffices = GetAvailableOffices();
            IEnumerable<IOffice> OfficeSuggestion;

            if (!PreferedNeighboorhoodOffices.Any()) OfficeSuggestion = SizeCapableOffices.Intersect(PreferedNeighboorhoodOffices);
            else OfficeSuggestion = SizeCapableOffices;

            OfficeSuggestion = OfficeSuggestion.Intersect(SuitableResourcesOffices).OrderBy(x => x.MaxCapacity).ThenBy(x=> x.AvailableResources.Count()).ThenByDescending(x => x.Name);
            OfficeSuggestion = OfficeSuggestion.Intersect(AvailableOffices);
            return OfficeSuggestion;
        }

        IEnumerable<IOffice> GetSizeCapableOffices(SuggestionRequest suggestionRequest)
        {
           return this.Offices.FindAll(PossibleOffice => PossibleOffice.MaxCapacity >= suggestionRequest.CapacityNeeded);
        }

        IEnumerable<IOffice> GetPreferedNeighboorhoodOffices(SuggestionRequest suggestionRequest)
        {
            if (suggestionRequest.PreferedNeigborHood == null) return this.Offices;

            List<OfficeSpecs> ResultOffices = new List<OfficeSpecs>();
            foreach (LocationSpecs location in Locations)
            {
                if (location.Neighborhood == suggestionRequest.PreferedNeigborHood)
                {
                    foreach(OfficeSpecs office in Offices)
                    {
                        if (location.Name == office.LocationName) ResultOffices.Add(office);
                    }
                }
            }
            return ResultOffices;
        }

        IEnumerable<IOffice> GetSuitableResourcesOffices(SuggestionRequest suggestionRequest)
        {
            if (!suggestionRequest.ResourcesNeeded.Any()) return this.Offices;

            List<OfficeSpecs> ResultOffices = new List<OfficeSpecs>();
            List<string> Resources = new List <string>();
            
            foreach (OfficeSpecs PossibleOffice in Offices)
            {
                foreach (string availableResource in PossibleOffice.AvailableResources)
                {
                    if (suggestionRequest.ResourcesNeeded.Contains(availableResource)) Resources.Add(availableResource);
                }
                if (Resources.Count() == suggestionRequest.ResourcesNeeded.Count()) ResultOffices.Add(PossibleOffice);
                Resources.Clear();    
            }
            return ResultOffices;
        }

        IEnumerable<IOffice> GetAvailableOffices()
        {
            if (!this.Bookings.Any()) return Offices;
            return this.Offices.FindAll(x => CheckIfOfficeisTaken(x.Name));
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