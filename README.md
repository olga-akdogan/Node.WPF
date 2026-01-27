# Node – MVP Astrology Dating App

Node is a WPF-based dating app MVP that explores how multiple external APIs can interact to generate astrology-based dating insights.

This version focuses on **API integration and data flow**, rather than advanced UI or social features.

---

## MVP Overview

In this MVP, users can:

- Create an account or log in
- Enter their birth data (date, time, and place)
- Generate an astrology-based **AI love profile**

The app combines geolocation, ephemeris calculations, and AI-generated interpretation into a single user flow.

---

## APIs Used

### Google Geocoding API
- Converts the user’s **birth place** into latitude and longitude
- Ensures accurate geographic coordinates for astrological calculations

### Ephemeris API
- Uses birth date, time, latitude, and longitude
- Retrieves planetary positions (natal chart data)
- Provides the astrological foundation for interpretation

### OpenAI API
- Consumes structured natal chart data
- Generates a **dating-app style love profile**
- Output is human-readable, warm, and practical
- No deterministic claims

---

## How It Works (Data Flow)

1. User enters birth date, time, and place  
2. Google Geocoding API converts place → latitude & longitude  
3. Ephemeris API calculates planetary positions  
4. Natal chart data is passed to OpenAI  
5. OpenAI generates a personalized love profile  
6. Result is displayed in the Profile view  

---

## Tech Stack

- **.NET / WPF** (desktop application)
- **Entity Framework Core** (SQLite)
- **MVVM architecture**
- **Dependency Injection** via Microsoft.Extensions.Hosting
- **HttpClient-based API integration**
- **User Secrets** for API keys (no secrets committed)

---

## MVP Focus

This MVP intentionally keeps the UI minimal.

Primary goals:
- Reliable API integration
- Clean separation of concerns (Services, ViewModels, Views)
- Secure handling of API keys
- End-to-end data flow from user input → AI output

---

## Future Features

Planned for future releases:

- User-to-user matching based on astrological compatibility
- Synastry analysis (Sun/Moon/Venus/Mars comparisons)
- Expanded user profiles (bio, hobbies, photos)
- Match-based messaging
- Improved UI/UX and visual astrology charts

---

## Notes

- This is an MVP / proof-of-concept
- Astrology interpretations are **not deterministic**
- No medical, psychological, or predictive claims are made

---

## Setup Notes

- API keys are stored using **.NET User Secrets**
- Required keys:
  - Google Geocoding API key
  - OpenAI API key
- Keys are **not included** in the repository

---

## Status

MVP complete  
Actively extensible

---

## Sources and References
- ChatGPT 5.2 for debugging 
- WPF Documentation: https://docs.microsoft.com/en-us/dotnet/desktop/wpf/
- ChatGPT API Documentation: https://platform.openai.com/docs/
- Google Geocoding API Documentation: https://developers.google.com/maps/documentation/geocoding/overview
- Ephemeris API Documentation:https://ephemeris.fyi/
- Several public repos:https://github.com/topics/dating-app
