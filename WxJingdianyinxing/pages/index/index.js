//index.js
//获取应用实例
const app = getApp()

Page({
  data: {
    motto: 'Hello World',
    userInfo: {},
    hasUserInfo: false,
    canIUse: wx.canIUse('button.open-type.getUserInfo'),
    bgImg: 'https://mioto.milbit.com/voteapp/bgImgIndex.png',
    userCode: '',
    status: 'checking'
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
  },
  getUserInfo: function (e) {
    console.log(e)
    app.globalData.userInfo = e.detail.userInfo
    this.setData({
      userInfo: e.detail.userInfo,
      hasUserInfo: true
    })
  },
  textInput: function (e) {
    this.setData({
      userCode: e.detail.value
    })
  },
  textConfirm: function (e) {
    this.setData({
      userCode: e.detail.value
    })
  },
  gotoVote: function () {
    var _this = this;
    if (!_this.data.userCode && _this.data.status == "checking")
      return;
    _this.setData({
      status: 'checking'
    })
    wx.showLoading({
      title: '验证中...',
      mask: true
    })
    wx.request({
      url: app.globalData.baseUrl + '/jdly/chekcUserCode',
      data: {
        userCode: _this.data.userCode
      },
      success: function (res) {
        if (res.data.success) {
          if (!res.data.msg) {
            wx.showToast({
              title: '工号不存在',
              icon: 'none'
            })
            return;
          }
          if (res.data.isVoted) {
            wx.showToast({
              title: '您已投票',
              icon: 'none'
            })
            return;
          }
          if (!res.data.isEnable) {
            wx.showToast({
              title: '活动未开启',
              icon: 'none'
            })
            return;
          }
          app.globalData.userCode = _this.data.userCode;
          wx.redirectTo({
            url: '/pages/vote/vote',
          })
        } else {
          wx.showToast({
            title: '验证失败',
            icon: 'none'
          })
        }
      },
      error: function () {
        wx.showToast({
          title: '验证失败',
          icon: 'none'
        })
      },
      complete: function () {
        _this.setData({
          status: ''
        })
      }
    })
  }
})
