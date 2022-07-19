# %% [markdown]
# ### Import libraries

from flask import Flask, request
from shallow_learning import read_data, cure_of_commas, split_data, train_knn, train_tree, train_forest

# %% [markdown]
# ### App

df = read_data('NewDataSaved.txt')
df = cure_of_commas(df)
X_train, y_train, X_test, y_test = split_data(df)
knn_clf, knn_acc = train_knn(X_train, y_train, X_test, y_test)
tree_clf, tree_acc = train_tree(X_train, y_train, X_test, y_test)
rfc, rfc_acc = train_forest(X_train, y_train, X_test, y_test)


app = Flask(__name__)


@app.route("/", methods=['OPTIONS'])
def discovery():
    return {
        'links': {
            '/retrain (GET)': 'Retrains all models on current data present.',
            '/knn-predict?input=[float x 10] (GET)': 'Predicts [AICanPlay, EnemyPlayed, iWon, enemyWon] using sklearn\'s knn classifier.',
            '/tree-predict?input=[float x 10] (GET)': 'Predicts [AICanPlay, EnemyPlayed, iWon, enemyWon] using sklearn\'s decision-tree classifier.',
            '/forest-predict?input=[float x 10] (GET)': 'Predicts [AICanPlay, EnemyPlayed, iWon, enemyWon] using sklearn\'s random-forest classifiers.'
        }
    }


@app.route("/retrain", methods=['GET'])
def retrain():
    global knn, knn_acc
    knn, knn_acc = train_knn(X_train, y_train, X_test, y_test)
    global tree_clf, tree_acc
    tree_clf, tree_acc = train_tree(X_train, y_train, X_test, y_test)
    global rfc, rfc_acc
    rfc, rfc_acc = train_forest(X_train, y_train, X_test, y_test)

    return {
        'message': 'Models were retrained on current data.'
    }


@app.route("/knn-predict", methods=['GET'])
def knn():
    args = request.args
    input = args.get('input').replace('[', '').replace(']', '').split(',')
    input = [float(x) for x in input]
    size = len(input)

    if size != 10:
        return {
            'message': 'Please provide a list of 10 floats as request input!'
        }

    y_pred = knn_clf.predict([input])

    return {
        'prediction': str(y_pred[0]),
        'predictionTupelLabels': ['AICanPlay', 'EnemyPlayed', 'iWon', 'enemyWon'],
        'accuracy': knn_acc
    }


@app.route("/tree-predict", methods=['GET'])
def tree():
    args = request.args
    input = args.get('input').replace('[', '').replace(']', '').split(',')
    input = [float(x) for x in input]
    size = len(input)

    if size != 10:
        return {
            'message': 'Please provide a list of 10 floats as request input!'
        }

    y_pred = tree_clf.predict([input])

    return {
        'prediction': str(y_pred[0]),
        'predictionTupelLabels': ['AICanPlay', 'EnemyPlayed', 'iWon', 'enemyWon'],
        'accuracy': tree_acc
    }


@app.route("/forest-predict", methods=['GET'])
def forest():
    args = request.args
    input = args.get('input').replace('[', '').replace(']', '').split(',')
    input = [float(x) for x in input]
    size = len(input)

    if size != 10:
        return {
            'message': 'Please provide a list of 10 floats as request input!'
        }

    y_pred = rfc.predict([input])

    return {
        'prediction': str(y_pred[0]),
        'predictionTupelLabels': ['AICanPlay', 'EnemyPlayed', 'iWon', 'enemyWon'],
        'accuracy': rfc_acc
    }
