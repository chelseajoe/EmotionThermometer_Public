# EmotionThermometer Project

Welcome to the EmotionThermometer Project repository! This project is a part of a research study aiming to understand and gauge childrens' emotional responses to different scenarios.

## Overview

The EmotionThermometer Project incorporates a sliding emotion thermometer, allowing users to choose between different emotions to express how they are feeling. The game also presents users with various scenarios that are read aloud to them, providing a platform to observe and analyze their responses.

## Features

- **Sliding Emotion Thermometer:** Thermometer wher users can slide and select different emotions to express their feelings. Scene: Emotional Thermometer
  
- **Scenario Presentation:** Scenarios are read aloud to users to elicit emotional responses. Scene: Scenario_ (1-3)
  
- **Firebase Backend:** Organized by PID number -> Session Number -> DATETIME + different emotions logged -> For each emotion, a strategy is logged

### Firebase Data Structure Example

```json
{
  "p40x": {
    "Session 1": {
      "DATETIME": "11/7/2023 11:39:10 PM",
      "Emotions": {
        "angry": {
          "Strategies": [
            "I try to go to sleep"
          ]
        },
        "nervous": {
          "Strategies": [
            "I count to ten",
            "I go and eat candy"
          ]
        }
      }
    }
  }
}
```

## Getting Started

To run the Emotion Thermometer Game locally, follow these steps:

1. Clone the repository:
   ```bash
   git clone [https://github.com/your-username/emotion-thermometer-game.git]
   ```
2. Firebase Database
   https://emotion-thermometer-default-rtdb.firebaseio.com/
