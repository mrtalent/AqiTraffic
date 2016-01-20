def get_paired_roads(s):
    if len(s) <= 1:
        return []
    if s[0][7:] == s[1][7:]:
        return [(s[0], s[1])] + get_paired_roads(s[2:])
    return get_paired_roads(s[1:])

def avg_speed(rid, ofst):
    return np.mean([speed[rid, datetime(2015,7,1) + timedelta(minutes=15 * ofst) + timedelta(days=i)] for i in xrange(183)])
srids = sorted(road_ids, key=lambda o: o[7:])
paired_roads = get_paired_roads(srids) 
for pr in paired_roads:
    plt.clf()
    x = [datetime(1994,12,2) + timedelta(minutes=15) * i for i in xrange(96)]
    y = [avg_speed(pr[0], i) for i in xrange(96)]
    plt.plot(x,y,'-', label=pr[0])
    y = [avg_speed(pr[1], i) for i in xrange(96)]
    plt.plot(x,y,'-', label=pr[1])
    plt.savefig('./tmp/' + pr[0][7:] + '.png')