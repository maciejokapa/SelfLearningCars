# SelfLearningCars [~2018]
A Unity3D project that simulates self learning cars.  
The project uses a custom made mix of basic machine learning algorithms (made by unexperienced student ;) ) such as simple neural networks and evolutionary algorithms.  

### Assumptions:
1. Each car is equipped with three sensors measuring the distance to the wall in the specific direction (yellow line).
   <img width="205" alt="single_car" src="https://github.com/maciejokapa/SelfLearningCars/blob/main/Screenshots/single_car.PNG">  
2. Cars ride on a track with the objective of riding as far as possible without colliding with a wall.
   <img width="339" alt="track" src="https://github.com/maciejokapa/SelfLearningCars/blob/main/Screenshots/track.PNG">
3. Performance of each car is measured in distance covered until the collision.
4. Current speed and direction of the car are calculated by simple network with one hidden layer.  

### Algorithm:
1. In first run 20 cars with random weigths are placed at a start.  
   <img width="251" alt="multiple_cars" src="https://github.com/maciejokapa/SelfLearningCars/blob/main/Screenshots/multiple_cars.PNG">  
3. When the run is over (every car colided with the wall), the performance of each car is measured.
4. New set of 20 cars is prepared. Two of them are winners of the previous run. Four - created by copying weigths from first and second place randomly. Eight - created by changing weights from the previous run by 10 to 50%. Remaining - created by assigning random weights.
5. New run with next generations is started.  
   <img width="408" alt="multiple_cars_2" src="https://github.com/maciejokapa/SelfLearningCars/blob/main/Screenshots/multiple_cars_2.PNG">
