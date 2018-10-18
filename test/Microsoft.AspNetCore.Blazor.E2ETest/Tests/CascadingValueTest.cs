// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using BasicTestApp;
using Microsoft.AspNetCore.Blazor.E2ETest.Infrastructure;
using Microsoft.AspNetCore.Blazor.E2ETest.Infrastructure.ServerFixtures;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Blazor.E2ETest.Tests
{
    public class CascadingValueTest : BasicTestAppTestBase
    {
        public CascadingValueTest(
            BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<Program> serverFixture,
            ITestOutputHelper output)
            : base(browserFixture, serverFixture, output)
        {
            Navigate(ServerPathBase, noReload: !serverFixture.UsingAspNetHost);
            MountTestComponent<BasicTestApp.CascadingValueTest.CascadingValueSupplier>();
        }
        
        [Fact]
        public void CanUpdateValuesMatchedByType()
        {
            var currentCount = Browser.FindElement(By.Id("current-count"));
            var incrementButton = Browser.FindElement(By.Id("increment-count"));

            // We have the correct initial value
            WaitAssert.Equal("100", () => currentCount.Text);

            // Updates are propagated
            incrementButton.Click();
            WaitAssert.Equal("101", () => currentCount.Text);
            incrementButton.Click();
            WaitAssert.Equal("102", () => currentCount.Text);

            // Didn't re-render unrelated descendants
            Assert.Equal("1", Browser.FindElement(By.Id("receive-by-interface-num-renders")).Text);
        }

        [Fact]
        public void CanUpdateValuesMatchedByName()
        {
            var currentFlag1Value = Browser.FindElement(By.Id("flag-1"));
            var currentFlag2Value = Browser.FindElement(By.Id("flag-2"));

            WaitAssert.Equal("False", () => currentFlag1Value.Text);
            WaitAssert.Equal("False", () => currentFlag2Value.Text);

            // Observe that the correct cascading parameter updates
            Browser.FindElement(By.Id("toggle-flag-1")).Click();
            WaitAssert.Equal("True", () => currentFlag1Value.Text);
            WaitAssert.Equal("False", () => currentFlag2Value.Text);
            Browser.FindElement(By.Id("toggle-flag-2")).Click();
            WaitAssert.Equal("True", () => currentFlag1Value.Text);
            WaitAssert.Equal("True", () => currentFlag2Value.Text);

            // Didn't re-render unrelated descendants
            Assert.Equal("1", Browser.FindElement(By.Id("receive-by-interface-num-renders")).Text);
        }

        [Fact]
        public void CanUpdateFixedValuesMatchedByInterface()
        {
            var currentCount = Browser.FindElement(By.Id("current-count"));
            var decrementButton = Browser.FindElement(By.Id("decrement-count"));

            // We have the correct initial value
            WaitAssert.Equal("100", () => currentCount.Text);

            // Updates are propagated
            decrementButton.Click();
            WaitAssert.Equal("99", () => currentCount.Text);
            decrementButton.Click();
            WaitAssert.Equal("98", () => currentCount.Text);

            // Renders the descendant the same number of times we triggered
            // events on it, because we always re-render components after they
            // have an event
            Assert.Equal("3", Browser.FindElement(By.Id("receive-by-interface-num-renders")).Text);
        }
    }
}
