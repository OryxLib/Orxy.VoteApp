//app.js
App({
  onLaunch: function (options) {
    var _VoteType = options.query['type'] || 'Voting';
    this.globalData.type = _VoteType;
    this.globalData.bgImg = 'https://mioto.milbit.com/voteapp/' + _VoteType + '_Cover.png?v=' + Math.random()
    this.globalData.bgImgDefault = 'https://mioto.milbit.com/voteapp/' + _VoteType + '_In.png?v=' + Math.random()
    // 展示本地存储能力
    var logs = wx.getStorageSync('logs') || []
    logs.unshift(Date.now())
    wx.setStorageSync('logs', logs)
    var currentOpenId = '';
    var loginCallback;
    var _this = this;

    wx.getLocation({
      success: function (res) {
        _this.latitude = res.latitude
        _this.longitude = res.longitude
        var speed = res.speed
        var accuracy = res.accuracy
      },
    })

    // 登录 
    wx.login({
      success: res => {
        // console.log(res);
        wx.request({
          url: this.globalData.baseUrl + "/User/UseCode",
          data: {
            code: res.code
          },
          success: function (res_openid) {
            currentOpenId = res_openid.data;
            _this.globalData.openId = res_openid.data;
            if (loginCallback) {
              loginCallback(currentOpenId);
            }
          }
        })
        // 发送 res.code 到后台换取 openId, sessionKey, unionId
      }
    })
    // 获取用户信息
    wx.getSetting({
      withCredentials: true,
      success: res => {
        if (res.authSetting['scope.userInfo']) {
          // 已经授权，可以直接调用 getUserInfo 获取头像昵称，不会弹框
          wx.getUserInfo({
            success: res => {
              // console.log(res)
              // 可以将 res 发送给后台解码出 unionId
              this.globalData.userInfo = res.userInfo
              this.globalData.openId = currentOpenId;
              var _this = this;
              if (currentOpenId && res.userInfo) {
                wx.request({
                  url: _this.globalData.baseUrl + '/user/InfoUpload',
                  method: "post",
                  header: {
                    'content-type': 'application/x-www-form-urlencoded'
                  },
                  data: {
                    jsonUserInfo: JSON.stringify(res.userInfo),
                    openId: currentOpenId
                  }, success: function (res) {
                    _this.globalData.voteName = res.data.voteInfo.name;
                    _this.globalData.voteType = res.data.voteInfo.type;
                  }
                })
              } else {
                loginCallback = function (_openid) {
                  // wx.request({
                  //   url: _this.globalData.baseUrl + '/user/InfoUpload',
                  //   method: "post",
                  //   header: {
                  //     'content-type': 'application/x-www-form-urlencoded'
                  //   },
                  //   data: {
                  //     jsonUserInfo: JSON.stringify(res.userInfo),
                  //     openId: _openid
                  //   }
                  // })
                }
              }

              // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
              // 所以此处加入 callback 以防止这种情况
              if (this.userInfoReadyCallback) {
                this.userInfoReadyCallback(res)
              }
            }
          })
        }
      }
    })
    this.startGetMsg();
  },
  startGetMsg: function () {
    var _this = this;
    if (_this.globalData.waitNextVoteConnected) {
      return;
    }
    wx.connectSocket({
      url: 'wss://voteapp.oryxl.com/ws?key=userSocket' || 'ws://localhost:19737/ws?key=userSocket' || ''
    })
    wx.onSocketOpen(function (res) {
      _this.globalData.waitNextVoteConnected = true;
      setInterval(function () {
        wx.sendSocketMessage({
          data: ['test data'],
        })
      }, 20000)
    })
    wx.onSocketMessage(function (res) {
      var resultObj = JSON.parse(res.data);
      _this.globalData.endTimeStamp = resultObj.timestamp;
      switch (resultObj.cmd) {
        case 'startVote':
          if (!_this.globalData.voted) {
            _this.gotoVote()
          }
          // else {
          //   wx.redirectTo({
          //     url: '/pages/thanks/thanks',
          //   })
          // }
          break;
        case 'endVote':
          //if (!_this.globalData.voted)
          _this.gotoStopVote();
          break;
        case 'nextVote':
          _this.goBack()
          break;
      }
    })
    wx.onSocketError(function (res) {
      _this.globalData.waitNextVoteConnected = false
    })
    wx.onSocketClose(function (res) {
      _this.globalData.waitNextVoteConnected = false
    })
  },
  gotoVote: function () {
    wx.redirectTo({
      url: '/pages/vote/vote',
    })
  },
  gotoStopVote: function () {
    wx.redirectTo({
      url: '/pages/voteStop/voteStop',
    })
  },
  gotoRefresh: function (callBack) {
    var _this = this;
    wx.request({
      url: _this.globalData.baseUrl + '/vote/checkenable',
      success: function (res) {
        var resObj = res.data;
        if (resObj.success) {
          if (resObj.isEnable) {
            _this.gotoVote();
            app.globalData.endTimeStamp = resObj.enableTime;
          }
          if (callBack) {
            callBack(resObj.isEnable)
          }
        }
      }
    })
  },
  goBack: function () {
    wx.redirectTo({
      url: '/pages/index/index',
    })
  },
  globalData: {
    waitNextVoteConnected: false,
    userInfo: null,
    type: 'Voting',
    baseUrl: 'https://voteapp.oryxl.com' || 'http://localhost:19738' || '',
    bgImg: 'https://mioto.milbit.com/voteapp/Voting_Cover.png?v=' + Math.random(),
    bgImgDefault: 'https://mioto.milbit.com/voteapp/Voting_In.png?v=' + Math.random()
  }
})