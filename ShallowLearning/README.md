# Shallow learning for the Uno game

## Running the server

This repository includes a Dockerfile and a docker-compose.yml to quickly run a web-server on port 5000. The web server is able to make predictions upon input data using 3 shallow learning algorithms:

1. KNN (Classifier)
2. Decision Tree (Classifier)
3. Random Forest (Classifier)


### Run the following command from the app-directory

```console
docker-compose up
```

The web-server exposes a few routes to interact with the three algorithms. An "OPTIONS" request on the home-route "/" retrieves a list of all possible links:

OPTIONS: /

```json
{
    "links": {
        "/forest-predict?input=[0,0,0.234567901234568,0.0740740740740741,1,1,2,0,0,0] (GET)": "Predicts [AICanPlay, EnemyPlayed, iWon, enemyWon] using sklearn's random-forest classifiers.",
        "/knn-predict?input=[0,0,0.234567901234568,0.0740740740740741,1,1,2,0,0,0] (GET)": "Predicts [AICanPlay, EnemyPlayed, iWon, enemyWon] using sklearn's knn classifier.",
        "/retrain (GET)": "Retrains all models on current data present.",
        "/tree-predict?input=[0,0,0.234567901234568,0.0740740740740741,1,1,2,0,0,0] (GET)": "Predicts [AICanPlay, EnemyPlayed, iWon, enemyWon] using sklearn's decision-tree classifier."
    }
}
```
