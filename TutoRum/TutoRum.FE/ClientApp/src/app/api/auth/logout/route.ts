import { cookies } from "next/headers";
import { NextResponse } from "next/server";

// Named export for the POST request handler (Logout)
export async function DELETE(request: Request) {
  try {
    const cookieStore = cookies();

    // Remove the session token cookie
    const response = NextResponse.json(
      { message: "Logged out successfully" },
      { status: 200 }
    );

    cookieStore.delete("sessionToken");

    return response;
  } catch (error: any) {
    return NextResponse.json(
      { message: "Internal Server Error", error: error.message },
      { status: 500 }
    );
  }
}
