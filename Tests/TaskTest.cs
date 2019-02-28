using System;
using Xunit;
using Imperium_Incursions_Waitlist;

namespace Tests
{
    public class TaskTest
    {
        [Fact]
        public void IntervalInSeconds()
        {
            try
            {
                Task.IntervalInSeconds(23, 59, 2, () => { });
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }
            
        }

        [Theory]
        [InlineData(-1, 20, 1)]
        [InlineData(1, 60, 1)]
        public void IntervalInSeconds_Rejects(int hour, int sec, double interval)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Task.IntervalInSeconds(hour, sec, interval, () => { }));
        }

        [Fact]
        public void IntervalInMinutes()
        {
            try
            {
                Task.IntervalInMinutes(0, 42, 2, () => { });
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }

        }

        [Theory]
        [InlineData(-1, 20, 1)]
        [InlineData(1, 60, 1)]
        public void IntervalInMinutes_Rejects(int hour, int min, double interval)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Task.IntervalInMinutes(hour, min, interval, () => { }));
        }

        [Fact]
        public void IntervalInHours()
        {
            try
            {
                Task.IntervalInHours(1, 42, 2, () => { });
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }

        }

        [Theory]
        [InlineData(-1, 20, 1)]
        [InlineData(1, 60, 1)]
        public void IntervalInHours_Rejects(int hour, int min, double interval)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Task.IntervalInHours(hour, min, interval, () => { }));
        }

        [Fact]
        public void IntervalInDays()
        {
            try
            {
                Task.IntervalInDays(1, 42, 2, () => { });
            }
            catch (Exception ex)
            {
                Assert.True(false, "Expected no exception, but got: " + ex.Message);
            }

        }

        [Theory]
        [InlineData(-1, 20, 1)]
        [InlineData(1, 60, 1)]
        public void IntervalInDays_Rejects(int hour, int min, double interval)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Task.IntervalInDays(hour, min, interval, () => { }));
        }
    }
}

