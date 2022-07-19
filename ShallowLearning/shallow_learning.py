# %% [markdown]
# ### Import libraries

import os
import pandas as pd
from sklearn import tree
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score, mean_squared_error
from sklearn.svm import SVC
from sklearn.neighbors import KNeighborsClassifier
from sklearn.preprocessing import PolynomialFeatures
from sklearn.linear_model import LinearRegression

# %% [markdown]
# ### Read file

# df = pd.read_csv(os.getcwd() + '\\..\\data\\DataSaved.txt',
#                  sep='-;', names=['ColorEnemy percent', 'TypeEnemy percent', 'ColorDeck percent', 'TypeDeck percent', 'EnemyCardTotal', 'AIcardTotal', 'MovesAIcanMakeOnCad', 'ICouldPlayAfterTurn', 'EnemyCouldPlayAfterTurn'])


def read_data(file_name: str) -> pd.DataFrame:
    path = os.path.join(os.getcwd(), 'data', file_name)
    return pd.read_csv(path,
                       sep='-;', names=['ColorOfEnemyCard percent', 'TypeOfEnemyCard percent', 'ColorOfCardTotal percent', 'TypeOfCardTotal percent', 'EnemyCardTotal', 'AIcardTotal', 'PosMoves', 'IsWildCard', 'AddsMoreCards', 'IsSkipCard', 'AiCanPlay', 'EnemyPlayed', 'IWon', 'EnemyWon'])


df = read_data('NewDataSaved.txt')

# %%


def replace_comma(val): return float(val.replace(
    ',', '.')) if type(val) == str else val


def cure_of_commas(df: pd.DataFrame) -> pd.DataFrame:
    df = df.replace({',': '.'})
    data = df.to_dict()

    data = {key: {k: replace_comma(v) for k, v in val.items()}
            for key, val in data.items()}

    return pd.DataFrame(data)


df = cure_of_commas(df)

# %% [markdown]
# ### Split df into training and testing sets


def split_data(df: pd.DataFrame) -> tuple[list, list, list, list]:
    l = df.values.tolist()

    half = int(len(l)/2)
    quarter = int(half/2)
    three_quarters = half + quarter

    training = l[0:three_quarters]
    test = l[three_quarters:len(l)]

    X_train = [x[:10] for x in training]
    y_train = [x[10:14] for x in training]
    X_test = [x[:10] for x in test]
    y_test = [x[10:14] for x in test]

    return X_train, y_train, X_test, y_test


X_train, y_train, X_test, y_test = split_data(df)

# %% [markdown]
# ### Tree classifier


def train_tree(X_train: list, y_train: list, X_test: list, y_test: list) -> tuple[tree.DecisionTreeClassifier, float]:
    tree_clf = tree.DecisionTreeClassifier()
    tree_clf = tree_clf.fit(X_train, y_train)

    y_pred = tree_clf.predict(X_test)

    tree_acc = accuracy_score(y_test, y_pred)

    return tree_clf, tree_acc


tree_clf, tree_acc = train_tree(X_train, y_train, X_test, y_test)

print('Tree performance: ' + str(tree_acc))

# %% [markdown]
# ### Random forest classifier


def train_forest(X_train: list, y_train: list, X_test: list, y_test: list) -> tuple[RandomForestClassifier, float]:
    rfc = RandomForestClassifier()
    rfc = rfc.fit(X_train, y_train)

    y_pred = rfc.predict(X_test)

    rfc_acc = accuracy_score(y_test, y_pred)

    return rfc, rfc_acc


rfc, rfc_acc = train_forest(X_train, y_train, X_test, y_test)

print('Forest performance: ' + str(rfc_acc))

# %% [markdown]
# ### KNeighborsClassifier


def train_knn(X_train: list, y_train: list, X_test: list, y_test: list) -> tuple[KNeighborsClassifier, float]:
    knn = KNeighborsClassifier(6)
    knn = knn.fit(X_train, y_train)

    y_pred = knn.predict(X_test)

    knn_acc = accuracy_score(y_test, y_pred)

    return knn, knn_acc


knn, knn_acc = train_knn(X_train, y_train, X_test, y_test)

print('KNN performance: ' + str(knn_acc))

# %% [markdown]
# ### SVM classifier

# svc = SVC()
# svc = svc.fit(X_train, y_train)

# y_pred = svc.predict(X_test)

# svm_acc = accuracy_score(y_test, y_pred)

# print('Performance: ' + str(svm_acc))
