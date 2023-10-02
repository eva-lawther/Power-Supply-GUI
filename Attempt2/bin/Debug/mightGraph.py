# Take a csv file and plot it using matplotlib

import matplotlib as mp


import pandas as pd
mp.plyplot.rcParams["figure.figsize"] = [7.00, 3.50]
mp.plyplot.rcParams["figure.autolayout"] = True
columns = ["Output", "Input"]
df = pd.read_csv("input.csv", usecols=columns)
print("Contents in csv file:", df)
mp.plyplot.plot(df.Name, df.Marks)
mp.plyplot.show()