import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

const privatePaths = ["/user", "/admin"];
const authPaths = ["/login", "/signup"];

// Regex cho posts/add và posts/{id}/edit
const postEditRegex = /^\/posts\/\d+\/edit$/; // Bất kỳ số nào giữa /posts/ và /edit
const postAddRegex = /^\/posts\/add$/; // Route /posts/add

// Middleware logic
export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;
  const sessionToken = request.cookies.get("sessionToken")?.value;

  console.log(sessionToken);

  // Chưa đăng nhập thì không cho vào private paths
  if (privatePaths.some((path) => pathname.startsWith(path)) && !sessionToken) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  if (pathname.startsWith("/admin") && sessionToken == "user") {
    return NextResponse.redirect(new URL("/forbidden", request.url));
  }

  // Đăng nhập rồi thì không cho vào login/signup
  if (authPaths.some((path) => pathname.startsWith(path)) && sessionToken) {
    return NextResponse.redirect(
      new URL("/user/settings/user-profile", request.url)
    );
  }

  if (pathname == "/user/settings/") {
    return NextResponse.redirect(
      new URL("/user/settings/user-profile", request.url)
    );
  }

  // Kiểm tra regex cho edit post hoặc add post
  if (
    (pathname.match(postEditRegex) || pathname.match(postAddRegex)) &&
    !sessionToken
  ) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  return NextResponse.next();
}

// Cấu hình matcher để bao gồm các đường dẫn cho posts/{id}/edit và posts/add
export const config = {
  matcher: ["/user/:path*", "/login", "/signup", "/admin/:path*"],
};
