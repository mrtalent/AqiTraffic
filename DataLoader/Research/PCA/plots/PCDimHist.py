﻿speed_data_byroads=[speed_seg(i) for i in road_ids]
y=[]
for X in speed_data_byroads:
    y.append(PCDim(X))
plt.hist(y)
plt.show()