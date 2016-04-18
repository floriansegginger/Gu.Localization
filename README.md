# Gu.Localization
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) 
[![Build status](https://ci.appveyor.com/api/projects/status/ili1qk8amyjmd71t?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-localization)

## Table of Contents
- [1. Usage in XAML.](#1-usage-in-xaml)
- [2. Usage in code](#2-usage-in-code)
- [3. Validation](#3-validation)
- [4. Error formats](#4-error-formats)
- [5. LanguageSelector](#5-languageselector)

## 1. Usage in XAML.

The library has a `StaticExtension` markupextension that is used when translating.
The reason for naming it `StaticExtension` and not `TranslateExtension` is that Resharper provides intellisense when named `StaticExtension`
Binding the text like below updates the text when `Translator.CurrentCulture`changes enabling runtime selection of language.

```
<Window ...
        xmlns:p="clr-namespace:Gu.Wpf.Localization.Demo.WithResources.Properties"
        xmlns:l="http://gu.se/Localization">
    ...
    <TextBlock Text="{l:Static p:Resources.SomeResource}" />
    <TextBlock Text="{l:Enum ResourceManager={x:Static p:Resources.ResourceManager}, 
                             Member={x:Static local:SomeEnum.SomeMember}}" />    
    ...
```

## 2. Usage in code.
```
Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
string translated = Translator<Properties.Resources>.Translate(nameof(Properties.Resources.SomeResource));
string translated = TranslatorTranslate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeResource));
Translation translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeResource))
```

## 3. Validation.
Conveience API for unit testing localization. The methods are not optimized for performance and loads all resources for all cultures into memory.

Validate a `ResourceManager` like this:
```
var errors = Validate.Translations(Properties.Resources.ResourceManager);
Assert.IsTrue(errors.IsEmpty);
```

Checks:
- That all keys has a non null value for all cultures in `Translator.AllCultures`
- If the resource is a format string like `"First: {0}, second{1}"` it checks that.
  - The number of format items are the same for all cultures.
  - That all format strings has format items numbered 0..1..n

Validate an `enum` like this:
```
var errors = Validate.EnumTranslations<DummyEnum>(Properties.Resources.ResourceManager);
Assert.IsTrue(errors.IsEmpty);
```
Checks:
- That all enum members has keys in the `ResourceManager`
- That all keys has non null value for all cultures in `Translator.AllCultures`

### 3.1. TranslationErrors
`errors.ToString("  ", Environment.NewLine);`
Prints a formatted report with the errors found, sample:

```
Key: EnglishOnly
  Missing for: { de, sv }
Key: Value___0_
  Has format errors, the formats are:
    Value: {0}
    null
    Värde: {0} {1}
```

## 4. Error formats
| Error               |  Format       |
|---------------------|:-------------:|
| missing key         |    `!{0}!`    |
| missing culture     |    `~{0}~`    |
| missing translation |    `_{0}_`    |
| missing resources   |    `?{0}?`    |
| invalid format      |`{{{0} : {1}}}`|
| unknown error       |    `#{0}#`    |

## 5. LanguageSelector
A simple control for changing current language.

`AutogenerateLanguages="True"` displays all cultures found in the running application and picks the default flag.
A few flags are included in the library, many are probably missing.

```
<l:LanguageSelector AutogenerateLanguages="True" />
```

Or

```
<l:LanguageSelector>
    <l:Language Culture="de-DE"
                FlagSource="pack://application:,,,/Gu.Wpf.Localization;component/Flags/de.png" />
    <l:Language Culture="en-GB"
                FlagSource="pack://application:,,,/Gu.Wpf.Localization;component/Flags/en.png" />                
    <l:Language Culture="sv-SE"
                FlagSource="pack://application:,,,/Gu.Wpf.Localization;component/Flags/sv.png" />
</l:LanguageSelector>
```

![screenie](http://i.imgur.com/DKfx8WB.png)
