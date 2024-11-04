"use client";

import React, { useEffect, useState } from 'react';
import Link from 'next/link';
import { apiGetRequest } from '@/app/api/getLoans/route';
import { getTotalLoanAmount } from '@/app/api/getTotalLoanAmount/route';

const Home = () => {
    const [loans, setLoans] = useState<{ LoanAmount: number; LoanId: number; LoanName: string; UserId: number }[]>([]);
    const [totalAmount, setTotalAmount] = useState<number | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [isClient, setIsClient] = useState(false); // Added flag to ensure rendering only happens on the client

    useEffect(() => {
        // Set flag to true after component mounts on the client
        setIsClient(true);

        // Fetch data only on the client side
        (async () => {
            try {
                const loansData = await apiGetRequest('/GetLoans');
                setLoans(loansData || []);

                const total = await getTotalLoanAmount();
                setTotalAmount(total || 0);
            } catch (error) {
                console.error('Error fetching data:', error);
                setError("Failed to load loan data. Please try again later.");
            }
        })();
    }, []);

    if (!isClient) {
        // Prevent rendering content that depends on client-side data during SSR
        return null;
    }

    return (
        <div className="body">
            <div className="title">
                <h1>Welcome to Loan Tracker</h1>
            </div>
            <div className="totalOwed">
                <h3>
                    Total Owed: ${totalAmount !== null ? totalAmount : 'Loading...'}
                </h3>
            </div>
            <div className="tableTitle">
                <h4>Loans</h4>
            </div>
            {error ? (
                <div className="error">{error}</div>
            ) : (
                <div className="loanTableContainer">
                    <table className="loanTable">
                        <thead>
                        <tr>
                            <th>Loan ID</th>
                            <th>User Id</th>
                            <th>Loan Name</th>
                            <th>Loan Amount</th>
                        </tr>
                        </thead>
                        <tbody>
                        {loans.length > 0 ? (
                            loans.map(({ LoanAmount, LoanId, LoanName, UserId }) => (
                                <tr key={LoanId}>
                                    <td>{LoanId}</td>
                                    <td>{UserId}</td>
                                    <td>{LoanName}</td>
                                    <td>${LoanAmount}</td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan={4}>No loans available</td>
                            </tr>
                        )}
                        </tbody>
                    </table>
                </div>
            )}
            <div className="buttonContainer">
                <Link href="/addLoan">
                    <button className="button">Add Loan</button>
                </Link>
            </div>
        </div>
    );
};

export default Home;