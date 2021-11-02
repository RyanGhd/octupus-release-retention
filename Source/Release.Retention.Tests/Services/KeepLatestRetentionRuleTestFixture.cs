using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Release.Retention.Test.Facilities;

namespace Release.Retention.Services
{
    public class KeepLatestRetentionRuleTestFixture
    {
        public AppReleaseBuilder DataBuilder { get; }



        static KeepLatestRetentionRuleTestFixture()
        {

        }
        public KeepLatestRetentionRuleTestFixture()
        {
            DataBuilder = new AppReleaseBuilder();
        }

        public static IEnumerable<object[]> ReleaseCreatedDates
        {
            get
            {
                var dt = new DateTime(2000, 1, 1);

                var data = new List<object[]>
                {
                    new object[]{dt,dt,1},
                    new object[]{dt.AddDays(1),dt,1},
                    new object[]{dt,dt.AddDays(1),1},

                    new object[]{dt,dt,2},
                    new object[]{dt.AddDays(1),dt,2},
                    new object[]{dt,dt.AddDays(1),2},
                };

                return data;
            }
        }
    }
}