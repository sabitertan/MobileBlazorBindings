﻿using Blaxamarin.Framework.Elements.Handlers;
using Emblazon;
using XF = Xamarin.Forms;

namespace Blaxamarin.Framework.Elements
{
#pragma warning disable CA1812 // Internal class that is apparently never instantiated; this class is instantiated generically
    internal class MasterDetailDetailPage : ContentPage
#pragma warning restore CA1812 // Internal class that is apparently never instantiated
    {
        static MasterDetailDetailPage()
        {
            ElementHandlerRegistry<IXamarinFormsElementHandler>
                .RegisterElementHandler<MasterDetailDetailPage>(renderer => new DetailPageHandler(renderer, new XF.ContentPage()));
        }
    }
}