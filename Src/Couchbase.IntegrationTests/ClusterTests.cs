﻿using System;
using System.Collections.Generic;
using System.Security.Authentication;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.IntegrationTests.Utils;
using Couchbase.N1QL;
using NUnit.Framework;

namespace Couchbase.IntegrationTests
{
    [TestFixture]
    public class ClusterTests
    {
        [Test]
        public void Test_Query()
        {
            var cluster = new Cluster(TestConfiguration.GetCurrentConfiguration());
            var credentials = new ClusterCredentials
            {
                BucketCredentials = new Dictionary<string, string>
                {
                    {"authenticated", "secret"}
                }
            };
            cluster.Authenticate(credentials);

            var result = cluster.Query<dynamic>("select * from authenticated limit 1;");
            Assert.AreEqual(QueryStatus.Success, result.Status);
        }

        [Test]
        public void Test_Query_WhenBucketOpen_Succeeds()
        {
            var cluster = new Cluster(TestConfiguration.GetCurrentConfiguration());
            var credentials = new ClusterCredentials
            {
                BucketCredentials = new Dictionary<string, string>
                {
                    {"authenticated", "secret"}
                }
            };
            cluster.Authenticate(credentials);
            cluster.OpenBucket("authenticated");

            var result = cluster.Query<dynamic>("select * from authenticated limit 1;");
            Assert.AreEqual(QueryStatus.Success, result.Status);
        }

        [Test]
        public void Test_Query_WhenBucketOpenedThatIsNotInAuthenticator_Fails()
        {
            var cluster = new Cluster(TestConfiguration.GetCurrentConfiguration());
            var credentials = new ClusterCredentials
            {
                BucketCredentials = new Dictionary<string, string>
                {
                    {"authenticated", "secret"}
                }
            };
            cluster.Authenticate(credentials);
            Assert.Throws<AuthenticationException>(() => cluster.OpenBucket("default"));
        }

        [Test]
        public void Test_Query_WhenInvalidPassword_ThrowsException()
        {
            var cluster = new Cluster(TestConfiguration.GetCurrentConfiguration());
            var credentials = new ClusterCredentials
            {
                BucketCredentials = new Dictionary<string, string>
                {
                    {"authenticated", "secret1"}
                }
            };
            cluster.Authenticate(credentials);

            Assert.Throws<AggregateException>(()=>cluster.Query<dynamic>("select * from authenticated limit 1;"));
        }

        [Test]
        public void Test_Query_WhenAuthenticateNotCalled_ThrowsInvalidOperationException()
        {
            var cluster = new Cluster(TestConfiguration.GetCurrentConfiguration());
            var credentials = new ClusterCredentials
            {
                BucketCredentials = new Dictionary<string, string>
                {
                    {"authenticated", "secret1"}
                }
            };

            Assert.Throws<InvalidOperationException>(() => cluster.Query<dynamic>("select * from authenticated limit 1;"));
        }

        #region KV Error Map

        [TestCase(true)]
        [TestCase(false)]
        [Ignore("Only supported on cluster 5.0+ or CouchbaseMock")]
        public void UseKvErrorMap_Retuns_True_When_KVErrorMap_Is_Enabled(bool enabled)
        {
            var config = TestConfiguration.GetConfiguration("basic");
            config.BucketConfigs = new Dictionary<string, BucketConfiguration>
            {
                {
                    "default", new BucketConfiguration
                    {
                        UseKvErrorMap = enabled
                    }
                }
            };

            var cluster = new Cluster(config);
            var bucket = cluster.OpenBucket("default");
            Assert.AreEqual(enabled, bucket.SupportsKvErrorMap);
        }

        #endregion
    }
}

#region [ License information          ]

/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2015 Couchbase, Inc.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/

#endregion
