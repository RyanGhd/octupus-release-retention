using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Release.Retention.Test.Facilities;
using Xunit;

// ReSharper disable InconsistentNaming

namespace Release.Retention.Domain.Model.Objects
{
    public class AppRelease_Tests
    {
        private readonly AppDeploymentBuilder _builder = new AppDeploymentBuilder();

        [Fact]
        public void Object_only_keeps_the_latest_deployments_per_environment()
        {
            // arrange
            var e1 = new AppEnvironment("1", "1");
            var e2 = new AppEnvironment("2", "2");
            var dt = DateTime.Now;

            var d1 = _builder.Start().WithEnv(e1).WithDeployedAt(dt.AddDays(-3)).Build();
            var d2 = _builder.Start().WithEnv(e1).WithDeployedAt(dt.AddDays(-2)).Build();
            var d3 = _builder.Start().WithEnv(e1).WithDeployedAt(dt.AddDays(-1)).Build();

            var d4 = _builder.Start().WithEnv(e2).WithDeployedAt(dt.AddDays(-3)).Build();
            var d5 = _builder.Start().WithEnv(e2).WithDeployedAt(dt.AddDays(-2)).Build();
            var d6 = _builder.Start().WithEnv(e2).WithDeployedAt(dt.AddDays(-1)).Build();

            // act
            var sut = new AppRelease("r1", "p-1", "v-1", DateTime.Now.AddDays(-5), new List<AppDeployment> { d1, d2, d3, d4, d5, d6 });

            // assert 
            Assert.True(sut.LatestDeploymentsPerEnvironment.Count() == 2);
            Assert.Contains(d3, sut.LatestDeploymentsPerEnvironment);
            Assert.Contains(d6, sut.LatestDeploymentsPerEnvironment);
        }


        [Fact]
        public void Object_can_be_built_without_any_deployment()
        {
            // act
            var sut = new AppRelease("r1", "p-1", "v-1", DateTime.Now.AddDays(-5), null);

            // assert 
            Assert.True(!sut.LatestDeploymentsPerEnvironment.Any());
        }
    }

}
