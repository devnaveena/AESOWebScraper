# AESO Wind Energy Scraper

A C# console application that scrapes real-time wind energy generation data from Alberta's electricity market (AESO).

## Description

This application downloads live electricity data from the AESO Current Supply Demand Report and extracts wind energy generation information, saving it to a CSV file.

## Features

- Downloads real-time data from AESO website
- Extracts WIND energy assets only
- Parses asset names, IDs, and generation values
- Exports data to CSV format

## Requirements

- .NET 9.0
- HtmlAgilityPack NuGet package

## Installation

1. Clone this repository
2. Navigate to the project directory
3. Restore dependencies:
   ```bash
   dotnet restore
   ```

## Usage

Run the application:
```bash
dotnet run
```

The application will generate a `WindGeneration.csv` file with the following format:

```csv
LastUpdate,Asset,AssetID,TotalNetGeneration
Dec 17, 2025 04:54,Castle Rock Ridge 2,CRR2,145.2
Dec 17, 2025 04:54,Summerview 1,IEW1,89.7
```

## Output Format

- **LastUpdate**: Timestamp when the data was last updated
- **Asset**: Name of the wind energy asset
- **AssetID**: Short code identifier (extracted from parentheses)
- **TotalNetGeneration**: Power generation in MW

## How It Works

1. Downloads HTML from AESO website
2. Parses HTML using HtmlAgilityPack
3. Locates the WIND section in the report
4. Extracts asset name, ID, and generation values
5. Saves data to CSV file

## Technologies Used

- C# .NET 9.0
- HtmlAgilityPack (HTML parsing)
- Regex (pattern matching)
- HttpClient (web requests)

## License

MIT
