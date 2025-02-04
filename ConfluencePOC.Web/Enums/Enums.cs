using System.Runtime.Serialization;

namespace ConfluencePOC.Web.Enums;

public enum PageType
{
    Guide,
    Advice
}

public enum PageWidth
{
    [EnumMember(Value = "Two Thirds")]
    TwoThirds,
    [EnumMember(Value = "Full Width")]
    FullWidth
}

public enum ContentWidth
{
    [EnumMember(Value = "One Third")]
    OneThird,
    [EnumMember(Value = "Two Thirds")]
    TwoThirds,
    [EnumMember(Value = "Full Width")]
    FullWidth
}

public enum GridType
{
    Cards,
    [EnumMember(Value = "Alternating Image and Text")]
    AlternatingImageAndText,
    [EnumMember(Value = "External Links")]
    ExternalLinks,
    Banner
}

public enum HeadingType
{
    [Obsolete("Only use H2 and below", true)]
    H1,
    H2,
    H3,
    H4,
    H5,
    H6
}

public enum BackgroundColour
{
    Blue,
    Grey,
    Green
}

public enum CallToActionSize
{
    Small,
    Large
}