﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class Config : DbModel {
        public string PlayerName { get; set; }
        public string CurrentLeague { get; set; }
        public bool OnlyShowOffersOfCurrentLeague { get; set; }
        public bool FilterSoldOffers { get; set; }
        public string SoldWhisper { get; set; }
        public string StillInterestedWhisper { get; set; }
        public string BusyWhisper { get; set; }
        public string ThanksWhisper { get; set; }
    }
}
