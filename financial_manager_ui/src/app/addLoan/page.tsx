"use client";

import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import { apiPostRequest } from '@/app/api/addLoan/route';

const AddLoan = () => {
    const [loanName, setLoanName] = useState('');
    const [loanAmount, setLoanAmount] = useState<number | ''>(''); // Updated to accept number or empty string
    const [userId, setUserId] = useState<number | ''>(''); // Updated to accept number or empty string
    const [loanId, setLoanId] = useState<number | ''>(''); // Updated to accept number or empty string
    const [errorMessage, setErrorMessage] = useState<string | null>(null); // State for error message

    const router = useRouter();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setErrorMessage(null); // Reset error message on form submission

        // Validate input fields
        if (userId === '' || loanId === '' || !loanName || loanAmount === '') {
            setErrorMessage("All fields are required.");
            return;
        }
        if (userId <= 0) {
            setErrorMessage("User ID must be a positive number.");
            return;
        }
        if (loanId <= 0) {
            setErrorMessage("Loan ID must be a positive number.");
            return;
        }
        if (loanAmount <= 0) {
            setErrorMessage("Loan Amount must be a positive number.");
            return;
        }

        const newLoan = {
            LoanId: loanId,
            UserId: userId,
            LoanName: loanName,
            LoanAmount: loanAmount,
        };

        try {
            const result = await apiPostRequest('/AddLoan', newLoan);
            if (result) {
                router.push('/');
            } else {
                setErrorMessage("Failed to add the loan. Please try again.");
            }
        } catch (error) {
            console.error("Error adding loan:", error);
            setErrorMessage("An error occurred while adding the loan. Please try again later.");
        }
    };

    return (
        <div className="bodyLoan">
            <div className="container">
                <h2>Add Loan</h2>
                {errorMessage && <div className="error">{errorMessage}</div>} {/* Display error message */}
                <form onSubmit={handleSubmit}>
                    <label className="label">User ID:</label>
                    <input
                        className="input"
                        type="number"
                        placeholder="User ID"
                        value={userId}
                        onChange={(e) => setUserId(e.target.value === '' ? '' : parseInt(e.target.value))}
                    />

                    <label className="label">Loan ID:</label>
                    <input
                        className="input"
                        type="number"
                        placeholder="Loan ID"
                        value={loanId}
                        onChange={(e) => setLoanId(e.target.value === '' ? '' : parseInt(e.target.value))}
                    />

                    <label className="label">Loan Name:</label>
                    <input
                        className="input"
                        type="text"
                        placeholder="Loan Name"
                        value={loanName}
                        onChange={(e) => setLoanName(e.target.value)}
                    />

                    <label className="label">Loan Amount:</label>
                    <input
                        className="input"
                        type="number"
                        placeholder="Loan Amount"
                        value={loanAmount}
                        onChange={(e) => setLoanAmount(e.target.value === '' ? '' : parseFloat(e.target.value))}
                    />

                    <button type="submit" className="buttonAddLoan">ADD</button>
                </form>
            </div>
        </div>
    );
};

export default AddLoan;