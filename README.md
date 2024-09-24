# Driver Situation Awareness Assessment

Situation Awareness (SA) is one of the core concepts describing drivers’ interaction with vehicles, and the lack of SA
has contributed to multiple incidents with automated systems. Despite existing definitions and measurements, little is
known about what constitutes the concept of situations from users’ perspective, i.e., do they have a similar or
different understanding of situation dynamics? Therefore, we conducted a video-based experiment where participants had
to mark the onset of new situations from their perspective, provide a continuous criticality rating, and justify their
decisions in a post-test interview. Our results indicate that the understanding of situations, their complexity, and
their duration is quite diverse between people and independent of properties such as age, gender, or driving experience,
while partly being influenced by the road type. Additionally, we found correlations between subjective situation
durations, criticality ratings, and algorithm output, which can be exploited by future applications and experiments.

## Table of Contents
- [Introduction](#introduction)
- [Citation](#citation)
- [Installation](#installation)
- [Usage](#usage)
- [Application](#application)
- [Results/Data](#resultsdata)
- [Contact Information](#contact-information)
- [Link to Study](#link-to-study)

## Introduction

This project is a Unity application developed in order to conduct a study on situation awareness in the context of
driving. The study aims to investigate the characteristics of situations from the perspective of the participants. The
application presents the participants with a series of videos showing driving scenarios and asks them to mark the onset
of new situations, provide a continuous criticality rating, and justify their decisions in a post-test interview.

## Citation
Please cite our paper as:

```
@article{asteriou2024situationAwarness,
  title={What Characterizes "Situations" in Situation Awareness? Findings from a Human-centered Investigation},
  author={Asteriou, Kotsios and Wintersberger},
  booktitle={Proceedings of the 16th International Conference on Automotive User Interfaces and Interactive Vehicular Applications},
  pages={216--226},
  year={2024},
  publisher = {Association for Computing Machinery},
  doi = {https://doi.org/10.1145/3640792.3675722}
}
```

## Installation

1. **Clone the repository:**

   ```bash
    git clone https://github.com/reapphil/DriverSituationAwarenessAssessment.git
   ```
2. **Open the project in Unity:**

- Launch Unity Hub
- Click on "Open" and navigate to the cloned repository folder
- Select the project folder to load it into Unity

## Usage

- Build the project:
    - Go to File > Build Settings
    - Select the target platform (e.g., Windows, Mac)
    - Click on the "Build" button to create the executable
- Place the video files in the BuildFolder\SituationStudy_Data\StreamingAssets\Videos directory
- Run the executable to start the application

## Application 

The application consist of 5 stages:

1. **Participant Name Input:**
    - The participant is asked to enter their name before starting the study.
    - The next sage is started by pressing the button on the controller.
2. **Video Playback:**
    - The participant is presented with a video showing a driving scenario.
    - The participant can mark the onset of a new situation by pressing the button on the controller.
    - The participant can provide a continuous criticality rating by using the dial on the controller.
3. **Situation Playback:**
    - The participant is presented with 6 random marked situations with a 5 second buffer before and after the marked
      situation. The Criticality rating is displayed on the screen as well.
    - The situation replays endlessly until the participant presses the button on the controller.
4. **Post-Test Interview:**
    - The participant is asked to fill out a Survey.


## Results/Data

The calculated results of the study are located in the JaspFiles/ directory.

## Contact Information

Author: Philipp Asteriou<br>
Email: asteriouphilipp@gmail.com<br>
Institution/Organization: FH Upper Austria University of Applied Sciences

# Link to Study
Access the full study [here](https://dl.acm.org/doi/abs/10.1145/3640792.3675722): 
