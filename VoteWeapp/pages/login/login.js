// pages/login/login.js
var app = getApp();
Page({
  data: {
    _title: '',
    userInfo: {},
    hasUserInfo: false,
    canIUse: wx.canIUse('button.open-type.getUserInfo'),
    options: [],
    bgImg: app.globalData.bgImg
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    console.log(options)
    this.setData({
      bgImg: app.globalData.bgImg,
    })
    // this.setData({
    //   title:options.title
    // })
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },

  formSubmit: function (e) {
    console.log('form发生了submit事件，携带数据为：', e.detail.value)
    if (e.detail.value.password) {
      wx.request({
        url: app.globalData.baseUrl + '/user/login', //仅为示例，并非真实的接口地址 
        data: {
          nickname: app.globalData.userInfo.nickName,
          username: e.detail.value.id,
          description: e.detail.value.password,
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
          var timeStamp = parseInt(res.data.voteStartTime) - new Date();
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
    } else {

    }
  },
})