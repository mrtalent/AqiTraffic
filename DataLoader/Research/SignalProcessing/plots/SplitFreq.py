# freq spliting in PERCENTAGE
y=speed_seq(road_ids[1])[1]
pieces=10
plt.subplot(pieces+1,1,1)
plt.plot(y)
for i in xrange(pieces):
    plt.subplot(pieces+1,1,i+2)
    plt.plot(subfreq(y, i*1.0/pieces, (i+1.0)/pieces))


# freq spliting in NUMBER
plt.clf()
y=speed_seq(road_ids[1])[1]
pieces=10
plt.subplot(pieces+1,1,1)
plt.plot(y)
for i in xrange(pieces):
    plt.subplot(pieces+1,1,i+2)
    plt.plot(subfreq2(y, i, i+1))