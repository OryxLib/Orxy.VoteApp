//index.js
//获取应用实例
const app = getApp()

Page({
  data: {
    motto: '欢迎',
    userInfo: {},
    hasUserInfo: false,
    canIUse: wx.canIUse('button.open-type.getUserInfo'),
    options: [],
    bgImg: app.globalData.bgImg,
    voteName: app.globalData.voteName,
    voteName1: '',
    voteName2: '',
    displayTitle: '',
    shouldLogin: false,
    emCode: ''
  },
  //事件处理函数
  bindViewTap: function () {
    wx.navigateTo({
      url: '../logs/logs'
    })
  },
  onLoad: function () {
    if (app.globalData.userInfo) {
      this.setData({
        userInfo: app.globalData.userInfo,
        hasUserInfo: true
      })
    } else if (this.data.canIUse) {
      // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
      // 所以此处加入 callback 以防止这种情况
      app.userInfoReadyCallback = res => {
        this.setData({
          userInfo: res.userInfo,
          hasUserInfo: true
        })
      }
    } else {
      // 在没有 open-type=getUserInfo 版本的兼容处理
      wx.getUserInfo({
        success: res => {
          app.globalData.userInfo = res.userInfo
          this.setData({
            userInfo: res.userInfo,
            hasUserInfo: true
          })
        }
      })
    }
    var _this = this;
    wx.request({
      url: app.globalData.baseUrl + '/vote/getInfo',
      method: "get",
      success: function (res) {
        console.log(res)
        app.globalData.voteName = res.data.question;
        app.globalData.type = res.data.VoteType;
        _this.setData({
          displayTitle: '/img/' + res.data.VoteType + '.png',
          shouldLogin: res.data.ShouldLogin
        })
        app.globalData.bgImg = 'https://mioto.milbit.com/voteapp/' + app.globalData.type + '_Cover.png?v=' + Math.random()
        app.globalData.bgImgDefault = 'https://mioto.milbit.com/voteapp/' + app.globalData.type + '_In.png?v=' + Math.random()
        console.log(app.globalData.bgImg)
        _this.setData({
          bgImg: app.globalData.bgImg,
          voteName: app.globalData.voteName,
          voteName1: app.globalData.voteName.split('/')[0],
          voteName2: app.globalData.voteName.split('/')[1]

        })
      }
    })
  },
  getUserInfo: function (e) {
    console.log(e)
    app.globalData.userInfo = e.detail.userInfo
    this.setData({
      userInfo: e.detail.userInfo,
      hasUserInfo: true
    })
  },
  staffNumInput: function (e) {
    this.setData({
      emCode: e.detail.value
    })
  },
  goVote: function () {
    var _this = this;
    if (this.data.shouldLogin) {
      if (!this.data.emCode) {
        return;
      }
      wx.request({
        url: app.globalData.baseUrl + '/user/login', //仅为示例，并非真实的接口地址 
        data: {
          nickname: app.globalData.userInfo.nickName,
          description: _this.data.emCode,
          openId: app.globalData.openId
        },
        header: {
          'content-type': 'application/json' // 默认值
        },
        success: function (res) {
          console.log(res.data)
          if (!res.data.success) {
            wx.showToast({
              title: '验证失败'
            })
            return;
          }

          app.startGetMsg();
          var timeStamp = parseInt(res.data.voteStartTime + res.data.closeSeconds * 1000) - new Date();
          app.globalData.endTimeStamp = res.data.voteStartTime
          if (res.data.isVoted) {
            wx.redirectTo({
              url: '/pages/thanks/thanks',
            })
            return;
          }
          if (res.data.isEnable && timeStamp < 0) {
            wx.redirectTo({
              url: '/pages/voteTimeout/voteTimeout',
            })
          }
          if (timeStamp > 0 || !res.data.isEnable) {
            wx.redirectTo({
              url: '/pages/voteWaiting/voteWaiting?timeStamp=' + timeStamp,
            })
          }
          else {
            wx.redirectTo({
              url: '../vote/vote?userId=' + e.detail.value.id
            })
          }

        }
      })
    }
    else {
      app.startGetMsg();
      wx.request({
        url: app.globalData.baseUrl + '/vote/CheckVoted',
        data: {
          userId: app.globalData.openId
        },
        success: function (res) {
          if (res.data.success) {
            if (!res.data.voted) {

              if (res.data.isEnable) {
                app.globalData.endTimeStamp = res.data.enableTime;
                wx.redirectTo({
                  url: '/pages/vote/vote',
                })
              } else {
                wx.redirectTo({
                  url: '/pages/voteWaiting/voteWaiting',
                })
              }
            } else {
              wx.redirectTo({
                url: '/pages/thanks/thanks',
              })
            }
          }
        }
      })
    }
  },
  goWxq: function () {
    wx.redirectTo({
      url: '/pages/wxq/wxq',
    })
  }
})
