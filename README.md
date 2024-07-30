# AI for Games Project: 3D Maze Game

This repository contains the AI for Games project developed during a course at the University of Limerick. The main concept of our game is a 3D maze environment where the main character must find the exit while avoiding or fighting enemies. AI is implemented on both the main character and the enemies, making the game a simulation where they play against each other. The project includes implementations of vision and hearing for all characters, and uses finite-state machines for intelligent behavior.

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [AI Implementation](#ai-implementation)
- [Links](#links)
- [Contributions](#contributions)

## Introduction

The project features a 3D maze game where the main character navigates through a maze, interacts with objects, and combats enemies using AI-driven behaviors. This simulation was created as part of the "AI for Games" course.

## Features

- 3D maze environment
- AI-driven main character and enemies
- Vision and hearing capabilities for characters
- Finite-state machine for character behavior
- Key and door mechanism

## AI Implementation

Maze Structure
The maze is composed of multiple square-shaped cells, each made by four walls. The maze was created by removing some walls from these cells and is represented as a grid in Unity.

DFS Maze Exploration
The Depth-First Search (DFS) algorithm is used to explore the maze. The main character starts at the [0,0] cell and navigates through the maze to find the exit, avoiding dead ends and backtracking when necessary.

Key and Door Mechanism
A key and door mechanism is implemented where the character must find a key to open a door within the maze. If the character finds the door without the key, it continues to explore until the key is found.

Hearing Mechanism
The hearing mechanism uses raycasts and Physics.OverlapSphere to detect sounds within a certain radius. The character adjusts its behavior based on the presence of enemies detected by sound.

Finite State Machine
The main character uses a finite state machine with two states: Explore and Attack. The Explore state includes pathfinding and maze navigation, while the Attack state handles combat with enemies.

Attack State
In the Attack state, the character engages with enemies based on distance and health comparison. The character attacks enemies if it has a chance to win based on its health.


## Links
Video explaining the main features of the game implemented by me and showing the gameflow of it. In t

https://www.youtube.com/watch?v=12kJtl-df58&t=3s

## Contributions

This project was done in collaboration with a classmate of mine.
