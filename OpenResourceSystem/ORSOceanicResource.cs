﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenResourceSystem {
    public class ORSOceanicResource {
        protected string resourcename;
        protected double abundance;
        protected string displayname;

        public ORSOceanicResource(string resourcename, double abundance, string displayname) {
            this.resourcename = resourcename;
            this.abundance = abundance;
            this.displayname = displayname;
        }

        public string getDisplayName() {
            return displayname;
        }

        public string getResourceName() {
            return resourcename;
        }

        public double getResourceAbundance() {
            return abundance;
        }
    }
}
