// Import the functions you need from the SDKs you need
import { initializeApp } from "firebase/app";
import { getStorage } from "firebase/storage";
import {
  getMessaging,
  getToken,
  isSupported,
  onMessage,
} from "firebase/messaging";

// Your web app's Firebase configuration
const firebaseConfig = {
  apiKey: "AIzaSyAVpDjktmoYol0yr2204135nSAV01w0NTc",
  authDomain: "tutorconnect-27339.firebaseapp.com",
  projectId: "tutorconnect-27339",
  storageBucket: "tutorconnect-27339.appspot.com",
  messagingSenderId: "1044368668051",
  appId: "1:1044368668051:web:c073d2158262dffd72e466",
  measurementId: "G-KZRCVW7ML8",
};

// Initialize Firebase App
const app = initializeApp(firebaseConfig);
export const storage = getStorage(app);

// Check if messaging is supported and initialize it
export const message = async () => {
  if (await isSupported()) {
    return getMessaging(app);
  } else {
    console.warn("Firebase Messaging không được hỗ trợ trên trình duyệt này.");
    return null;
  }
};

// Request notification permission
export const requestNotificationPermission = async () => {
  try {
    const permission = await Notification.requestPermission();
    if (permission === "granted") {
      console.log("Quyền nhận thông báo được cấp.");
    } else if (permission === "denied") {
      console.warn("Người dùng từ chối nhận thông báo.");
    } else {
      console.warn("Quyền nhận thông báo chưa được quyết định.");
    }
  } catch (error) {
    console.error("Lỗi khi yêu cầu quyền nhận thông báo:", error);
  }
};

// Get FCM Token
export const getFcmToken = async () => {
  try {
    const messaging = await message();
    if (!messaging) return null;

    const vapidKey =
      "BO0-fsEeRS7s8Vj9lgsKhRMGKOIlcS5bj2BKZfplb9XgWjmAJz0KTktmR1_G2_pyEAysDxwt_jOnDr6RqhAU9ic";

    const token = await getToken(messaging, { vapidKey });
    if (token) {
      console.log("FCM Token nhận được:", token);
      return token; // Gửi về backend
    } else {
      console.warn("Không thể lấy được FCM Token.");
      return null;
    }
  } catch (error) {
    console.error("Lỗi khi lấy FCM Token:", error);
    return null;
  }
};

// Listen for messages
export const listenForMessages = async () => {
  const messaging = await message();
  if (!messaging) return;

  onMessage(messaging, (payload) => {
    console.log("Thông báo nhận được:", payload);

    // Xử lý hiển thị thông báo bằng toast (hoặc UI khác)
    if (payload.notification) {
      const { title, body } = payload.notification;
      // Sử dụng toast của bạn để hiển thị
      console.log("Hiển thị toast:", title, body);
    }
  });
};
