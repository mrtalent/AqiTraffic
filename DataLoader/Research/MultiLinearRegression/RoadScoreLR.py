def paint(t=10, cmap='Blues'):
    res = gencases(t=t)
    tmp = []
    for cnt, data in enumerate(res):
        lm = sklm.LinearRegression()
        X,y = data[0],np.array(data[1])[:, None]
        lm.fit(X, y)
        tmp.append((road_ids[cnt], lm.score(X,y)))
    scale = min(tmp, key=lambda o:o[1])[1], max(tmp, key=lambda o:o[1])[1]
    color_map = plt.get_cmap(cmap)
    for i in tmp:
        rid = i[0]
        score = i[1]
        for seg in road[rid]['coords']:
            x = [i[0] for i in seg]
            y = [i[1] for i in seg]
            plt.plot(x, y, '-', color=color_map((score - scale[0]) * 0.8 / (scale[1] - scale[0]) + 0.2), lw=2)
    plt.xlim(116.0, 116.8)
    plt.ylim(39.65, 40.3)
    plt.show()
    return tmp