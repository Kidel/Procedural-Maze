#Procedural Maze
An example game made with Unity where the map is a procedurally generated maze.

Requires unity Standard Assets and Unity Engine. 

* Demo: http://proceduralmaze.azurewebsites.net

## Example
The function MazeGenerator.Generate() returns a char[,] of the given maze sizes (from class constructor). 
The maze is mostly random and looks like this: 
```
###################################################
#########           #######   #####################
#########   ####### ####### # #####################
######### & - ##### ####### #                     #
#########   # ##### ####### ##################### #
#       #   #       ####### #   #####           # #
#       ##-## #####-####### # # ##### ######### # #
#       #  ## #####       #   # ##### ######### # #
#       # ### #####       #-### ##### ######### # #
#   &   # #   #####       #   # #     ######### # #
#   &   # # #######    &  # & - # ############# # #
#    &  - # #######       #   # # ############# # #
#       # # #######   &   ##### # ############# # #
#       # # #######   &   ##### # ############# # #
######### # #######  &    ##### # ############# # #
#           #######    &  ##### # #############   #
# #################       ##### # #################
# #               #       ##### # ########### -   #
# # ####### ##### #       ##### # ########### #   #
# # ####### ##### #       ##### #             # & #
# # ####### ##### ##-########## ############# #   #
# # ####### ##### #             #     #     # #   #
# # ####### ##### # #############     - ### # ##-##
# # ####### ##### # ###   #     #  &  # ### # ##  #
# # ####### ##### # ### # # ### #     # ### # ### #
# # ####### ##### #     # # ### #     # ### # ### #
# # ####### ##### ####### # ### ##-#-## ### # ### #
# # ####### #####         # ### ##      ### # ### #
# # ####### ############### ### ########### # ### #
# # ####### #     # -     # ###             # ### #
# # ####### #     - #     # ################# ### #
# #     ### #  &  # #  &  -                       #
# # ###-### #  @  # #     # ###############-#-#####
# # #     # #     - #  &  # #             #       #
# # #     # #     # #     # # ########### #       #
# # #  &  # #  &  # #     # # ########### #       #
# # #  &  # #     # ####-## # ########### #   &&  #
# # #  &  # #     #       # # ########### #  & &  #
# # #  &  # ####### ##### # # ########### #       #
# # #  &  #       # ##### # #       ##### #    &  #
# # #     ####### # ##### # ####### ##### #       #
# # #     ####### # ##### # ####### #     #       #
# # ###-######### # ##### # ####### # ###-##### ###
# # #   ######### #     # # ####### # ###     # ###
# # # ########### ##### # # ####### #-###     # ###
# # # ########### -   # # # ####### #   #  &  # ###
# # # ########### #   # # # ####### #   #  &  # ###
# #   ########### # & # # # ####### # & #     - ###
# ############### #   - # # ####### #   -     #####
#                 #   ###           #   #     #####
###################################################
```
Where spaces are floor, '#' are walls, '-' are doors, '&' are enemies and '@' is the player. 
Characters used can be changed in Tiles. 

Finally MazeGeneratorBehaviour is a MonoBehaviour class that generates a physical maze from prefabs using the structure provided by MazeGenerator. 

Various options allow to create different mazes, like this one (with dead ends):
```
###################################################
# #       -             #                         #
# # # ### #     ####### # ######### ############# #
# # # # # -     #     # #         # #           # #
# # # # # #     # ### # #-####### # ####-#### # # #
# # # #   #     #   # # #   #   # # -       # # # #
# ### ########-###### # #   # # # # #       # # # #
#     #     #         # #   - # # # #       # # # #
#-#####     - ####### # #   # ### # #       # # # #
#     -  &  # #       # #   #     # #       # # # #
#     #     # ######### ##-######## #       # # # #
#     #     #           #           #       # # # #
#     ####### ############# ################# ### #
#     #             #     # #                     #
#     #####-##-#### #     # ######### ########### #
#     #           # #  @  - #       #       #   # #
#     #           # #     # #       #-##### #   - #
#     #           # #     # -       #   # # # & # #
# ##-##           # ##-#-## #       #   # # #   # #
# #   #           #         #       -   #   #   # #
# # # #           #-#########       #   ######-## #
# # # #           #       # #       #   #       # #
# # # #           #       # ###-#-####-##       - #
# # # #           #       #   -   #     #       # #
# # # #           #       # # # & - ### #       # #
# # # #           #       # # #   # # # #  &    # #
# ### #           #       # # ##### # # #       # #
#     #  &        #       - #     # # # #       # #
#-### #           #       # ####### # # #####-### #
#   # #           #       #           #         # #
# & ##################-#### ########### ######### #
#   -     #               # #         #           #
# ###     # ######### ### # #         #############
# # -     # # #     # # # # #         # #         #
# # #     # # #     # # # # #         # #         #
# # #     # # #     # # # # -         # #         #
# # #     # # #     # # # # #         # #      &  #
#   #     # # -     # #   # #         # -         #
#-######-## # #     # ##### #         # #         #
#     #   # # #     #       #         # #         #
#     # # # # ##-############         - #         #
#     # # # #         #     #         # #         #
#     # # # # ###-### #     #-######### #####-#####
#     # # # # # #   # -  &  #   #                 #
#     # ### # # # & - #     #   # ### ########### #
#     #     # # #   # #     - & # # # -     #   # #
#     # ##### # ##-## #     #   # # # #     - # # #
#     # #           # #     #   # # # #     # # # #
##-#### ############# ##-######## # # #     # ### #
#                                 #   #     #     #
###################################################
```
