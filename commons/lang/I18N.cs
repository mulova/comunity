using System;
using System.Globalization;

public class I18N
{
	/// <summary>
	/// Gets the currency.
	/// </summary>
	/// <returns>The currency.</returns>
	/// <param name="language">such as </param>
	public static string GetCurrency(string language) {
		try {
			RegionInfo region = new RegionInfo(CultureInfo.CreateSpecificCulture(language).ToString());
			return region.CurrencySymbol;
		} catch {
			return "USD";
		}
	}

	public static string GetLanguage() {
		return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
	}

	public static string GetCurrency() {
		return RegionInfo.CurrentRegion.CurrencySymbol;
	}

	public static string GetCountry() {
		return RegionInfo.CurrentRegion.TwoLetterISORegionName;
	}
}
