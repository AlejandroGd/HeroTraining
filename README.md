# HeroTraining

While studying my degree I got a special interest in artificial intelligence. I saw the ability to train your characters as an interesting idea as a game concept. This was a project that would make me gain some more insight into learning A.I. I also wanted to improve my proficiency with Unity, so this was the perfect opportunity for it.

Hero Training is a 2D turn-based RPG game where you can select a team of 2 heroes to battle monsters. For each team, you choose one hero to control and one that will use its A.I. to choose which action to perform in its turn.

Records of every time a player has used a skill with the character are kept in that character's A.I., along with different parameters about the state of the battle when the skill was chosen (the health of all characters, their attributes, positive or negative effects active, etc) and used to build a decision tree to act as A.I. for that character.

To build the decision tree, the ID3 algorithm is used. Starting with a root node containing all records, the algorithm recursively splits nodes into branches that contain instances with similar values (homogenous), using the entropy concept to calculate the homogeneity of the candidate nodes. The process of building the decision tree is based on finding the attribute that split the nodes into the most homogeneous branches.

A group of players were asked to try the game and it was found out that healing was the easier skill to teach, followed by the type of attack to use (use the right element attack according to monster weaknesses) while support actions (like using shields or status inducing skills) were difficult to get right. The research conducted at initial stages of the project suggested that this could be solved by "pruning" the tree to avoid poor generalization in specific record sets or maybe use some kind of indirect methods (like reinforcement learning or optimisation methods) that are better at dealing with unseen cases.

By researching for this project I learnt a lot about different methods to build a decision making A.I. and really cool stuff that has been going around in other games in terms of A.I. that learns by imitation. It definitely tested my skills with Unity and allowed me to deepen my knowledge about Unity and game programming.

I revisited this project to refactor the Fight Battle system, which was implemented in a very complex way at the time. Now it is more straightforward and the logic can be followed easily.
