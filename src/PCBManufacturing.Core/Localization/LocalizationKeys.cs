namespace PCBManufacturing.Core.Localization;

// Compile-time keys for ILocalizationService: _text[Order.Error.Project].
// Each const value is the JSON key. A name ending in Hint, Value, or Description
// stands for a dotted segment when the shorter key is already a const
// (Field.Postcode and Field.PostcodeHint). A leading underscore marks a
// numeric segment (Thickness._08 is "Thickness.08").

public static class App
{
    public const string Title = "App.Title";
    public const string Subtitle = "App.Subtitle";
}

public static class Tab
{
    public const string Preferences = "Tab.Preferences";
    public const string Quote = "Tab.Quote";
    public const string Order = "Tab.Order";
}

public static class Status
{
    public const string Saved = "Status.Saved";
    public const string Dirty = "Status.Dirty";
}

public static class Action
{
    public const string Save = "Action.Save";
    public const string Discard = "Action.Discard";
    public const string Continue = "Action.Continue";
    public const string PlaceOrder = "Action.PlaceOrder";
    public const string Submitting = "Action.Submitting";
    public const string Confirm = "Action.Confirm";
    public const string Cancel = "Action.Cancel";
    public const string Close = "Action.Close";
    public const string Theme = "Action.Theme";
    public const string Language = "Action.Language";
}

public static class Theme
{
    public const string Light = "Theme.Light";
    public const string Dark = "Theme.Dark";
}

public static class Preferences
{
    public const string Heading = "Preferences.Heading";
    public const string Intro = "Preferences.Intro";
}

public static class Field
{
    public const string Project = "Field.Project";
    public const string Postcode = "Field.Postcode";
    public const string PostcodeHint = "Field.Postcode.Hint";
    public const string Material = "Field.Material";
    public const string Mask = "Field.Mask";
    public const string Thickness = "Field.Thickness";
    public const string Quantity = "Field.Quantity";
    public const string Notes = "Field.Notes";
}

public static class Summary
{
    public const string Title = "Summary.Title";
    public const string Project = "Summary.Project";
    public const string Postcode = "Summary.Postcode";
    public const string Material = "Summary.Material";
    public const string Modifier = "Summary.Modifier";
    public const string Lead = "Summary.Lead";
    public const string Mask = "Summary.Mask";
    public const string Thickness = "Summary.Thickness";
    public const string Quantity = "Summary.Quantity";
    public const string Notes = "Summary.Notes";
    public const string Valid = "Summary.Valid";
    public const string Invalid = "Summary.Invalid";
    public const string Empty = "Summary.Empty";
    public const string Days = "Summary.Days";
}

public static class Quote
{
    public const string Heading = "Quote.Heading";
    public const string Intro = "Quote.Intro";
    public const string Parameters = "Quote.Parameters";
    public const string Preview = "Quote.Preview";
    public const string Layers = "Quote.Layers";
    public const string Breakdown = "Quote.Breakdown";
    public const string Unit = "Quote.Unit";
    public const string Total = "Quote.Total";
    public const string Lead = "Quote.Lead";
    public const string Base = "Quote.Base";
    public const string Size = "Quote.Size";
    public const string Material = "Quote.Material";
    public const string Thickness = "Quote.Thickness";
    public const string LayersLine = "Quote.LayersLine";
    public const string Finish = "Quote.Finish";
    public const string Quantity = "Quote.Quantity";
    public const string Incomplete = "Quote.Incomplete";

    public static class Column
    {
        public const string Parameter = "Quote.Column.Parameter";
        public const string Value = "Quote.Column.Value";
        public const string Time = "Quote.Column.Time";
        public const string Cost = "Quote.Column.Cost";
    }

    public static class Legend
    {
        public const string Mask = "Quote.Legend.Mask";
        public const string Finish = "Quote.Legend.Finish";
        public const string Layers = "Quote.Legend.Layers";
        public const string Size = "Quote.Legend.Size";
    }
}

public static class Order
{
    public const string Heading = "Order.Heading";
    public const string Intro = "Order.Intro";
    public const string Place = "Order.Place";
    public const string EmptyTitle = "Order.EmptyTitle";
    public const string EmptyBody = "Order.EmptyBody";
    public const string History = "Order.History";
    public const string Number = "Order.Number";
    public const string Created = "Order.Created";
    public const string DialogTitle = "Order.DialogTitle";
    public const string DialogMessage = "Order.DialogMessage";
    public const string ValidationTitle = "Order.ValidationTitle";
    public const string ValidationMessage = "Order.ValidationMessage";
    public const string Confirmed = "Order.Confirmed";
    public const string Notes = "Order.Notes";
    public const string FailureTitle = "Order.FailureTitle";
    public const string MakeCurrent = "Order.MakeCurrent";

    public static class Error
    {
        public const string Project = "Order.Error.Project";
        public const string Postcode = "Order.Error.Postcode";
        public const string Material = "Order.Error.Material";
        public const string Mask = "Order.Error.Mask";
        public const string Thickness = "Order.Error.Thickness";
        public const string Quantity = "Order.Error.Quantity";
        public const string Size = "Order.Error.Size";
        public const string Layers = "Order.Error.Layers";
        public const string Finish = "Order.Error.Finish";
    }
}

public static class Validation
{
    public static class Postcode
    {
        public const string Empty = "Validation.Postcode.Empty";
        public const string Invalid = "Validation.Postcode.Invalid";
    }

    public static class Project
    {
        public const string Empty = "Validation.Project.Empty";
    }
}

public static class Impact
{
    public const string None = "Impact.None";
    public const string Days = "Impact.Days";
    public const string Modifier = "Impact.Modifier";
    public const string Included = "Impact.Included";
}

public static class Group
{
    public const string Dimensions = "Group.Dimensions";
    public const string Layers = "Group.Layers";
    public const string Finish = "Group.Finish";
    public const string Quantity = "Group.Quantity";
}

public static class Material
{
    public const string Fr4 = "Material.Fr4";
    public const string Fr4Description = "Material.Fr4.Description";
    public const string Aluminum = "Material.Aluminum";
    public const string AluminumDescription = "Material.Aluminum.Description";
    public const string Rogers = "Material.Rogers";
    public const string RogersDescription = "Material.Rogers.Description";
}

public static class Color
{
    public const string Green = "Color.Green";
    public const string Blue = "Color.Blue";
    public const string Red = "Color.Red";
    public const string Black = "Color.Black";
    public const string White = "Color.White";
    public const string Yellow = "Color.Yellow";
}

public static class Thickness
{
    public const string _08 = "Thickness.08";
    public const string _16 = "Thickness.16";
    public const string _24 = "Thickness.24";
}

public static class Param
{
    public const string Size50 = "Param.Size50";
    public const string Size50Value = "Param.Size50.Value";
    public const string Size100 = "Param.Size100";
    public const string Size100Value = "Param.Size100.Value";
    public const string Size160 = "Param.Size160";
    public const string Size160Value = "Param.Size160.Value";
    public const string Layers2 = "Param.Layers2";
    public const string Layers2Value = "Param.Layers2.Value";
    public const string Layers4 = "Param.Layers4";
    public const string Layers4Value = "Param.Layers4.Value";
    public const string Layers6 = "Param.Layers6";
    public const string Layers6Value = "Param.Layers6.Value";
    public const string Layers8 = "Param.Layers8";
    public const string Layers8Value = "Param.Layers8.Value";
    public const string Hasl = "Param.Hasl";
    public const string HaslValue = "Param.Hasl.Value";
    public const string Enig = "Param.Enig";
    public const string EnigValue = "Param.Enig.Value";
    public const string Osp = "Param.Osp";
    public const string OspValue = "Param.Osp.Value";
    public const string Qty5 = "Param.Qty5";
    public const string Qty5Value = "Param.Qty5.Value";
    public const string Qty10 = "Param.Qty10";
    public const string Qty10Value = "Param.Qty10.Value";
    public const string Qty25 = "Param.Qty25";
    public const string Qty25Value = "Param.Qty25.Value";
    public const string Qty100 = "Param.Qty100";
    public const string Qty100Value = "Param.Qty100.Value";
}
