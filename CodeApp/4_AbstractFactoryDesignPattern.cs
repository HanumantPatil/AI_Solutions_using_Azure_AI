using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeApp
{
    // UI Sysytem
    interface IButton
    {
        void RenderButton();
    }
    interface IToolBar
    {
        void RenderToolBar();
    }
    interface IBackground
    {
        void RenderBackground();
    }
    class LightThemeButton : IButton
    {
        public void RenderButton()
        {
            throw new NotImplementedException();
        }
    }

    class LightThemeToolBar : IToolBar
    {
        public void RenderToolBar()
        {
            throw new NotImplementedException();
        }
    }


    class LightThemeBackground : IBackground
    {
        public void RenderBackground()
        {
            throw new NotImplementedException();
        }
    }

    class DarkThemeButton : IButton
    {
        public void RenderButton()
        {
            throw new NotImplementedException();
        }
    }

    class DarkThemeToolBar : IToolBar
    {
        public void RenderToolBar()
        {
            throw new NotImplementedException();
        }
    }


    class DarkThemeBackground : IBackground
    {
        public void RenderBackground()
        {
            throw new NotImplementedException();
        }
    }
    interface ITheme
    {
        IButton RenderButton();

    }

    internal class _4_AbstractFactoryDesignPattern
    {
    }
}
