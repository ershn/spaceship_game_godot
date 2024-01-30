# TODO

## Game systems

- re-implement resource consumption/production

- add multi world
- add spaceship movement to arbitrary worlds
- limit the grid size of worlds
- add a storage furniture
- add a sweep order
- add a wall furniture

## Food consumption

- low priority: equally distribute food between clones when there isn't enough of it

## Path finding

## Item deliveries

- handle scenarios where an item is unreachable
- optimization: allow to pick several items during the same task
- optimization: pick the closest items and executors to fulfill a task

## Task scheduling

- add task priorities

## Camera

- allow to move the camera within a world
- allow to zoom in and out

## Graphics

### Clones

- add an eating animation
- add an idle animation

### Items

- fix the rendering order of items on the same tile

## Building

- allow to build a rectangular area of floors

## UX/UI

- allow to select which world layer to affect for cancel/deconstruct
- add an info window to show being/item/building properties

## Program architecture

- regroup StructureDef subclass specific methods in one place
  - \*Constructor.IsConstructibleAt

## Editor

- re-implement the generation of structure/item scenes from defs

### Logic graph editor

- add auto-generated descriptions to nodes
- prevent the removal of graph entrypoint nodes
- implement undo/redo
- prevent invalid connections (e.g. StateMachine output to normal method)
- skip invalid node indexes/connections on graph load

## Release

## Licenses
